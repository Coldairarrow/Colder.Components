using Colder.CommonUtil;
using Colder.MessageBus.Abstractions;
using Colder.MessageBus.Hosting;
using Colder.MessageBus.MQTT.Primitives;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Colder.MessageBus.MQTT
{
    internal class MqttMessageReceivedEventHandler
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly MessageBusOptions _messageBusOptions;
        private readonly IMqttClient _mqttClient;
        public MqttMessageReceivedEventHandler(
            IServiceProvider serviceProvider, 
            MessageBusOptions messageBusOptions,
            IMqttClient mqttClient
            )
        {
            _serviceProvider = serviceProvider;
            _messageBusOptions = messageBusOptions;
            _mqttClient = mqttClient;
        }
        public async Task Handle(MqttMessageReceivedEvent notification)
        {
            Topic topic = Topic.Parse(notification.Topic);
            string payload = Encoding.UTF8.GetString(notification.Payload);

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
