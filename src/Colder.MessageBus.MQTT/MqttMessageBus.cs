using Colder.MessageBus.Abstractions;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Colder.MessageBus.MassTransit
{
    internal class MqttMessageBus : IMessageBus
    {
        private readonly MessageBusOptions _options;
        private readonly IMqttClient _mqttClient;
        public MqttMessageBus( MessageBusOptions options, IMqttClient mqttClient)
        {
            _options = options;

            _mqttClient = mqttClient;
        }
        private CancellationToken GetCancellationToken()
        {
            return new CancellationTokenSource(TimeSpan.FromSeconds(_options.SendMessageTimeout)).Token;
        }
        //private Uri BuildUri(string endpoint)
        //{
        //    //return new Uri($"{_options.Host}{endpoint}");
        //}

        async Task IMessageBus.Publish<TMessage>(TMessage message, string endpoint)
        {
            var payload = new MqttApplicationMessageBuilder()
                .WithPayload(JsonConvert.SerializeObject(message))
                .WithAtLeastOnceQoS()
                .WithTopic("");

            await _mqttClient.PublishAsync(new MQTTnet.MqttApplicationMessage)
            //if (string.IsNullOrEmpty(endpoint))
            //{
            //    await _busControl.Publish(message, GetCancellationToken());
            //}
            //else
            //{
            //    var channel = await _busControl.GetSendEndpoint(BuildUri(endpoint));

            //    await channel.Send(message, GetCancellationToken());
            //}
        }
        async Task<TResponse> IMessageBus.Request<TRequest, TResponse>(TRequest message, string endpoint)
        {
            //var reqTimeout = RequestTimeout.After(0, 0, 0, _options.SendMessageTimeout);
            //var response = await _busControl.Request<TRequest, TResponse>(BuildUri(endpoint), message, default, reqTimeout);

            //return response.Message;
        }
    }
}
