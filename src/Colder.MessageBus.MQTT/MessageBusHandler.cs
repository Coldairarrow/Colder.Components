using Colder.CommonUtil;
using Colder.MessageBus.Abstractions;
using Colder.MessageBus.Hosting;
using Colder.MessageBus.MQTT.Primitives;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Receiving;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Colder.MessageBus.MQTT
{
    internal class MessageBusHandler : IMqttApplicationMessageReceivedHandler
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly MessageBusOptions _messageBusOptions;
        private readonly IMqttClient _mqttClient;
        private readonly ILogger _logger;
        public MessageBusHandler(
            IServiceProvider serviceProvider,
            MessageBusOptions messageBusOptions,
            IMqttClient mqttClient
            )
        {
            _serviceProvider = serviceProvider;
            _messageBusOptions = messageBusOptions;
            _mqttClient = mqttClient;
            _logger = _serviceProvider.GetService<ILogger<MessageBusHandler>>();
        }
        public async Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
        {
            //这里为单线程，暂时只能强行转为多线程
            //https://github.com/chkr1011/MQTTnet/issues/812
            //await Handle(e);
            _ = Task.Run(async () =>
            {
                try
                {
                    await Handle(e);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            });

            await Task.CompletedTask;
        }

        private async Task Handle(MqttApplicationMessageReceivedEventArgs e)
        {
            Topic topic = Topic.Parse(e.ApplicationMessage.Topic);
            string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

            //请求返回
            if (topic.MessageType == MessageTypes.Response)
            {
                if (RequestWaiter.WaitingDic.TryGetValue(topic.MessageId, out Waiter waiter))
                {
                    waiter.ResponseJson = payload;
                    waiter.Sp.Release();
                }

                return;
            }

            var theHandler = Cache.GetHandler(topic.MessageBodyType);

            if (theHandler.handleMessageType != null)
            {
                using var scop = _serviceProvider.CreateScope();

                var messageContext = Activator.CreateInstance(typeof(MessageContext<>).MakeGenericType(theHandler.handleMessageType)) as MessageContext;
                object message = JsonConvert.DeserializeObject(payload, theHandler.realMessageType);

                messageContext.ServiceProvider = scop.ServiceProvider;
                messageContext.MessageId = topic.MessageId;
                messageContext.MessageBody = payload;
                messageContext.SetPropertyValue("Message", message);

                var handlerInstance = ActivatorUtilities.CreateInstance(scop.ServiceProvider, theHandler.handlerType);

                var task = theHandler.handleMethod.Invoke(handlerInstance, new object[] { messageContext }) as Task;
                await task;

                //请求返回
                if (messageContext.Response != null)
                {
                    Topic responseTopic = new Topic
                    {
                        MessageId = topic.MessageId,
                        MessageBodyType = messageContext.Response.GetType().FullName,
                        MessageType = MessageTypes.Response,
                        SourceClientId = _mqttClient.Options.ClientId,
                        SourceEndpoint = _messageBusOptions.Endpoint,
                        TargetClientId = topic.SourceClientId,
                        TargetEndpoint = topic.SourceEndpoint
                    };

                    var responsePayload = new MqttApplicationMessageBuilder()
                        .WithPayload(JsonConvert.SerializeObject(messageContext.Response))
                        .WithAtLeastOnceQoS()
                        .WithTopic(responseTopic.ToString());

                    await _mqttClient.PublishAsync(responsePayload.Build());
                }
            }
        }
    }
}
