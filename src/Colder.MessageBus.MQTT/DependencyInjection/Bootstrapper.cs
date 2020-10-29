using Microsoft.Extensions.Hosting;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using System.Threading;
using System.Threading.Tasks;

namespace Colder.MessageBus.MQTT.DependencyInjection
{
    internal class Bootstrapper : BackgroundService
    {
        private readonly IMqttClient _mqttClient;
        private readonly IMqttClientOptions _mqttClientOptions;
        public Bootstrapper(IMqttClient mqttClient, IMqttClientOptions mqttClientOptions)
        {
            _mqttClient = mqttClient;
            _mqttClientOptions = mqttClientOptions;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _mqttClient.ConnectAsync(_mqttClientOptions);
        }
    }
}
