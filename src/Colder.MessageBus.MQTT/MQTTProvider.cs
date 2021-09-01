using Colder.Common;
using Colder.MessageBus.Abstractions;
using Colder.MessageBus.Hosting;
using Colder.MessageBus.MQTT.Primitives;
using MassTransit;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

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
            IBusControl busControl = null;

            var host = Options.Host.Split(':');

            var options = new MqttClientOptionsBuilder()
                .WithClientId(Guid.NewGuid().ToString())
                .WithTcpServer(host[0], int.Parse(host[1]))
                .Build();

            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();

            mqttClient.UseApplicationMessageReceivedHandler(async e =>
            {
                await busControl.Publish(new MqttMessageReceivedEvent
                {
                    Topic = e.ApplicationMessage.Topic,
                    Payload = e.ApplicationMessage.Payload,
                });
            });

            mqttClient.UseConnectedHandler(async e =>
            {
                //Topic格式
                //RootTopic/{SourceClientId}/{TargetClientId}/{SourceEndpoint}/{TargetEndpoint}/{MessageBodyType}/{MessageType}/{MessageId}

                string topic;
                foreach (var aMessageType in Cache.MessageTypes)
                {
                    //事件广播
                    topic = $"{Topic.RootTopic}/+/+/+/+/{aMessageType.FullName}/{MessageTypes.Event}/+";
                    await mqttClient.SubscribeAsync(topic);
                    Logger.LogInformation("MessageBus:Subscribe To Topic {Topic}", topic);

                    //命令单播
                    topic = $"{Topic.RootTopic}/+/+/+/{Options.Endpoint}/{aMessageType.FullName}/{MessageTypes.Command}/+";
                    await mqttClient.SubscribeAsync(topic);
                    Logger.LogInformation("MessageBus:Subscribe To Topic {Topic}", topic);

                    Logger.LogInformation("MessageBus:Subscribe {MessageType}", aMessageType);
                }

                //请求返回
                topic = $"{Topic.RootTopic}/+/{options.ClientId}/+/+/+/{MessageTypes.Response}/+";
                await mqttClient.SubscribeAsync(topic);
                Logger.LogInformation("MessageBus:Subscribe To Topic {Topic}", topic);
            });

            mqttClient.UseDisconnectedHandler(async e =>
            {
                Logger.LogWarning("MessageBus:Disconnected from {Host}", Options.Host);
                await Task.Delay(TimeSpan.FromSeconds(3));

                try
                {
                    await mqttClient.ConnectAsync(options, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "MessageBus:Reconnect To {Host} Fail", Options.Host);
                }
            });

            busControl = Bus.Factory.CreateUsingInMemory(sbc =>
            {
                sbc.ReceiveEndpoint(ep =>
                {
                    ep.Handler<MqttMessageReceivedEvent>(async context =>
                    {
                        await new MqttMessageReceivedEventHandler(ServiceProvider, Options, mqttClient)
                            .Handle(context.Message);
                    });
                });
            });
            busControl.Start();

            AsyncHelper.RunSync(() => mqttClient.ConnectAsync(options));
            Logger.LogInformation("MessageBus:Started (Host:{Host})", Options.Host);

            return new MqttMessageBus(Options, mqttClient);
        }
    }
}
