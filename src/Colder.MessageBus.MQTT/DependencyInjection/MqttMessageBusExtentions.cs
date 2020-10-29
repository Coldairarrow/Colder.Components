using Colder.MessageBus.Abstractions;
using Colder.MessageBus.MQTT.Primitives;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System;

namespace Colder.MessageBus.MQTT.DependencyInjection
{
    /// <summary>
    /// 拓展
    /// </summary>
    public static class MqttMessageBusExtentions
    {
        public static IServiceCollection AddMqttMessageBus(this IServiceCollection services, MessageBusOptions messageBusOptions)
        {
            var options = new MqttClientOptionsBuilder()
                .WithClientId($"{messageBusOptions.Endpoint}.{Environment.MachineName}")
                .WithTcpServer(messageBusOptions.Host)
                .Build();

            services.AddSingleton(options);

            services.AddSingleton(serviceProvider =>
            {
                var factory = new MqttFactory();
                var mqttClient = factory.CreateMqttClient();
                
                mqttClient.UseApplicationMessageReceivedHandler(e =>
                {
                    Topic topic = Topic.Parse(e.ApplicationMessage.Topic);

                    //Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                    //Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                    //Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                    //Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                    //Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
                    //Console.WriteLine();
                });

                mqttClient.UseConnectedHandler(async e =>
                {
                    //Console.WriteLine("### CONNECTED WITH SERVER ###");

                    //// Subscribe to a topic
                    //await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("my/topic").Build());

                    //Console.WriteLine("### SUBSCRIBED ###");
                });

                return mqttClient;
            });

            services.AddHostedService<Bootstrapper>();
            //await mqttClient.ConnectAsync(options, CancellationToken.None); // Since 3.0.5 with CancellationToken
            return services;
        }
    }
}
