using Colder.CommonUtil;
using Colder.MessageBus.Abstractions;
using Colder.MessageBus.MQTT.Primitives;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Colder.MessageBus.MQTT.DependencyInjection
{
    /// <summary>
    /// 拓展
    /// </summary>
    public static class MqttMessageBusExtentions
    {
        /// <summary>
        /// 注入MQTT消息总线
        /// </summary>
        /// <param name="services">服务容器</param>
        /// <param name="messageBusOptions">配置参数</param>
        /// <returns></returns>
        public static IServiceCollection AddMqttMessageBus(this IServiceCollection services, MessageBusOptions messageBusOptions)
        {
            var host = messageBusOptions.Host.Split(':');

            var options = new MqttClientOptionsBuilder()
                .WithClientId($"{messageBusOptions.Endpoint}.{Environment.MachineName}")
                .WithTcpServer(host[0], int.Parse(host[1]))
                .Build();

            services.AddSingleton(options);

            services.AddSingleton(serviceProvider =>
            {
                var factory = new MqttFactory();
                var mqttClient = factory.CreateMqttClient();

                mqttClient.UseApplicationMessageReceivedHandler(async e =>
                {
                    Topic topic = Topic.Parse(e.ApplicationMessage.Topic);
                    var theMessageType = Cache.MessageTypes.Where(x => x.FullName == topic.MessageBodyType).FirstOrDefault();
                    if (theMessageType != null)
                    {
                        using var scop = serviceProvider.CreateScope();

                        object messageContext = Activator.CreateInstance(typeof(MessageContext<>).MakeGenericType(theMessageType));
                        string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                        object message = JsonConvert.DeserializeObject(payload, theMessageType);

                        messageContext.SetPropertyValue("MessageId", topic.MessageId);
                        messageContext.SetPropertyValue("Message", message);
                        messageContext.SetPropertyValue("MessageBody", payload);

                        var handlerType = Cache.MessageHandlers[theMessageType];
                        var handler = ActivatorUtilities.CreateInstance(scop.ServiceProvider, handlerType);
                        var method = handler.GetType().GetMethods().Where(x =>
                             x.Name == "Handle"
                             && x.GetParameters().Length == 1
                             && x.GetParameters()[0].ParameterType == messageContext.GetType()
                            ).FirstOrDefault();

                        if (method != null)
                        {
                            var task = method.Invoke(handler, new object[] { messageContext }) as Task;
                            await task;
                        }
                    }
                });

                mqttClient.UseConnectedHandler(async e =>
                {
                    //Topic格式
                    //ClientId={Endpoint}.{MachineName}
                    //Colder.MessageBus.MQTT/{SourceClientId}/{TargetClientId}/{SourceEndpoint}/{TargetEndpoint}/{MessageBodyType}/{MessageType}/{MessageId}

                    foreach (var aMessageType in Cache.MessageTypes)
                    {
                        //事件广播
                        await mqttClient.SubscribeAsync(
                            $"{Topic.RootTopic}/+/+/+/+/{aMessageType.FullName}/Event/+");
                        //命令单播
                        await mqttClient.SubscribeAsync(
                            $"{Topic.RootTopic}/+/+/+/{messageBusOptions.Endpoint}/{aMessageType.FullName}/Command/+");
                        //请求返回
                        await mqttClient.SubscribeAsync(
                            $"{Topic.RootTopic}/+/{options.ClientId}/+/+/{aMessageType.FullName}/Response/+");
                    }
                });

                return mqttClient;
            });

            services.AddSingleton<IMessageBus>(serviceProvider =>
                new MqttMessageBus(messageBusOptions, serviceProvider.GetService<IMqttClient>()));

            services.AddHostedService<Bootstrapper>();
            return services;
        }
    }
}
