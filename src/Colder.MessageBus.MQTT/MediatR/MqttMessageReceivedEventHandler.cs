using Colder.CommonUtil;
using Colder.MessageBus.Abstractions;
using Colder.MessageBus.Hosting;
using Colder.MessageBus.MQTT.Primitives;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Colder.MessageBus.MQTT.MediatR
{
    class MqttMessageReceivedEventHandler : INotificationHandler<MqttMessageReceivedEvent>
    {
        public async Task Handle(MqttMessageReceivedEvent notification, CancellationToken cancellationToken)
        {
            Topic topic = Topic.Parse(notification.EventArgs.ApplicationMessage.Topic);
            string payload = Encoding.UTF8.GetString(notification.EventArgs.ApplicationMessage.Payload);

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
                using var scop = notification.ServiceProvider.CreateScope();

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
                        SourceClientId = notification.MqttClient.Options.ClientId,
                        SourceEndpoint = notification.MessageBusOptions.Endpoint,
                        TargetClientId = topic.SourceClientId,
                        TargetEndpoint = topic.SourceEndpoint
                    };

                    var responsePayload = new MqttApplicationMessageBuilder()
                        .WithPayload(JsonConvert.SerializeObject(messageContext.Response))
                        .WithAtLeastOnceQoS()
                        .WithTopic(responseTopic.ToString());

                    await notification.MqttClient.PublishAsync(responsePayload.Build());
                }
            }
        }
    }
}
