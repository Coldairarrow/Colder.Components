using Colder.Common;
using Colder.MessageBus.Abstractions;
using Colder.MessageBus.Hosting;
using Colder.MessageBus.MQTT.Primitives;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;

namespace Colder.MessageBus.MQTT
{
    internal class MQTTProvider : AbstractProvider
    {
        public MQTTProvider(IServiceProvider serviceProvider, MessageBusOptions options)
            : base(serviceProvider, options)
        {
        }

        public override IMessageBus GetBusInstance()
        {
            var host = Options.Host.Split(':');

            var options = new MqttClientOptionsBuilder()
                .WithClientId($"{Options.Endpoint}.{Environment.MachineName}")
                .WithTcpServer(host[0], int.Parse(host[1]))
                .Build();

            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();

            mqttClient.UseApplicationMessageReceivedHandler(
                new MessageBusHandler(ServiceProvider, Options, mqttClient));

            mqttClient.UseConnectedHandler(async e =>
            {
                //Topic格式
                //ClientId={Endpoint}.{MachineName}
                //Colder.MessageBus.MQTT/{SourceClientId}/{TargetClientId}/{SourceEndpoint}/{TargetEndpoint}/{MessageBodyType}/{MessageType}/{MessageId}

                foreach (var aMessageType in Cache.MessageTypes)
                {
                    //事件广播
                    await mqttClient.SubscribeAsync(
                        $"{Topic.RootTopic}/+/+/+/+/{aMessageType.FullName}/{MessageTypes.Event}/+");
                    //命令单播
                    await mqttClient.SubscribeAsync(
                        $"{Topic.RootTopic}/+/+/+/{Options.Endpoint}/{aMessageType.FullName}/{MessageTypes.Command}/+");
                    //请求返回
                    await mqttClient.SubscribeAsync(
                        $"{Topic.RootTopic}/+/{options.ClientId}/+/+/{aMessageType.FullName}/{MessageTypes.Response}/+");
                }
            });

            AsyncHelper.RunSync(() => mqttClient.ConnectAsync(options));

            return new MqttMessageBus(Options, mqttClient);
        }
    }
}
