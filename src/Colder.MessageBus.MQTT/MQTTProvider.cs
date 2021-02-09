using Colder.Common;
using Colder.MessageBus.Abstractions;
using Colder.MessageBus.Hosting;
using Colder.MessageBus.MQTT.Primitives;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly static IMediator _mediator;
        static MQTTProvider()
        {
            var services = new ServiceCollection();

            services.AddMediatR(typeof(MqttMessageReceivedEvent));

            _mediator = services.BuildServiceProvider().GetService<IMediator>();
        }

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
                new MessageBusHandler(ServiceProvider, Options, mqttClient, _mediator));

            mqttClient.UseConnectedHandler(async e =>
            {
                //Topic格式
                //ClientId={Endpoint}.{MachineName}
                //Colder.MessageBus.MQTT/{SourceClientId}/{TargetClientId}/{SourceEndpoint}/{TargetEndpoint}/{MessageBodyType}/{MessageType}/{MessageId}

                string topic;
                foreach (var aMessageType in Cache.AllMessageTypes)
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

            AsyncHelper.RunSync(() => mqttClient.ConnectAsync(options));
            Logger.LogInformation("MessageBus:Started (Host:{Host})", Options.Host);

            return new MqttMessageBus(Options, mqttClient);
        }
    }
}
