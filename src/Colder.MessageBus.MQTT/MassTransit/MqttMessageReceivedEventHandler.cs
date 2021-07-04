using Colder.Common;
using Colder.MessageBus.Abstractions;
using Colder.MessageBus.Hosting;
using Colder.MessageBus.MQTT.Primitives;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using System;
using System.Linq;
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

            var messageType = Cache.MessageTypes.Where(x => x.FullName == topic.MessageBodyType).FirstOrDefault();
            if (messageType == null)
            {
                return;
            }

            if (Cache.Message2Handler.TryGetValue(messageType, out Type theHandlerType))
            {
                using var scop = _serviceProvider.CreateScope();

                var messageContextType = typeof(MessageContext<>).MakeGenericType(messageType);
                var messageContext = Activator.CreateInstance(messageContextType) as MessageContext;
                object message = JsonConvert.DeserializeObject(payload, messageType);

                messageContext.ServiceProvider = scop.ServiceProvider;
                messageContext.MessageId = topic.MessageId;
                messageContext.MessageBody = payload;
                messageContext.SetPropertyValue("Message", message);

                var handlerInstance = ActivatorUtilities.CreateInstance(scop.ServiceProvider, theHandlerType);

                var method = theHandlerType.GetMethod("Handle", new Type[] { messageContextType });

                var task = method.Invoke(handlerInstance, new object[] { messageContext }) as Task;
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
