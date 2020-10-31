using Colder.MessageBus.Abstractions;
using Colder.MessageBus.MQTT.Primitives;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Colder.MessageBus.MQTT
{
    internal class MqttMessageBus : IMessageBus
    {
        private readonly MessageBusOptions _options;
        private readonly IMqttClient _mqttClient;
        public MqttMessageBus(MessageBusOptions options, IMqttClient mqttClient)
        {
            _options = options;

            _mqttClient = mqttClient;
        }
        private CancellationToken GetCancellationToken()
        {
            return new CancellationTokenSource(TimeSpan.FromSeconds(_options.SendMessageTimeout)).Token;
        }

        async Task IMessageBus.Publish<TMessage>(TMessage message, string endpoint)
        {
            Topic topic = new Topic
            {
                MessageId = Guid.NewGuid(),
                MessageBodyType = typeof(TMessage).FullName,
                MessageType = string.IsNullOrEmpty(endpoint) ? MessageTypes.Event : MessageTypes.Command,
                SourceClientId = _mqttClient.Options.ClientId,
                SourceEndpoint = _options.Endpoint,
                TargetClientId = "*",
                TargetEndpoint = string.IsNullOrEmpty(endpoint) ? "*" : endpoint,
            };

            var payload = new MqttApplicationMessageBuilder()
                .WithPayload(JsonConvert.SerializeObject(message))
                .WithAtLeastOnceQoS()
                .WithTopic(topic.ToString());

            await _mqttClient.PublishAsync(payload.Build());
        }

        async Task<TResponse> IMessageBus.Request<TRequest, TResponse>(TRequest message, string endpoint)
        {
            await Task.CompletedTask;

            throw new NotImplementedException();

            //var reqTimeout = RequestTimeout.After(0, 0, 0, _options.SendMessageTimeout);
            //var response = await _busControl.Request<TRequest, TResponse>(BuildUri(endpoint), message, default, reqTimeout);

            //return response.Message;
        }
    }
}
