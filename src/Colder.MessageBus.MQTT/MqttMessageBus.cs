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

        public async Task Publish<TMessage>(TMessage message, string endpoint) where TMessage : class
        {
            Topic topic = new Topic
            {
                MessageId = Guid.NewGuid(),
                MessageBodyType = message.GetType().FullName,
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

        public async Task<TResponse> Request<TRequest, TResponse>(TRequest message, string endpoint)
            where TRequest : class
            where TResponse : class
        {
            Topic topic = new Topic
            {
                MessageId = Guid.NewGuid(),
                MessageBodyType = message.GetType().FullName,
                MessageType = MessageTypes.Command,
                SourceClientId = _mqttClient.Options.ClientId,
                SourceEndpoint = _options.Endpoint,
                TargetClientId = "*",
                TargetEndpoint = endpoint
            };

            var payload = new MqttApplicationMessageBuilder()
                .WithPayload(JsonConvert.SerializeObject(message))
                .WithAtMostOnceQoS()
                .WithTopic(topic.ToString());

            await _mqttClient.PublishAsync(payload.Build());

            Waiter theWaiter = new Waiter { Sp = new Semaphore(0, 1) };
            RequestWaiter.WaitingDic[topic.MessageId] = theWaiter;

            bool success = theWaiter.Sp.WaitOne(TimeSpan.FromSeconds(_options.SendMessageTimeout));
            RequestWaiter.WaitingDic.TryRemove(topic.MessageId, out _);

            if (!success)
            {
                throw new TimeoutException("请求超时");
            }

            return JsonConvert.DeserializeObject<TResponse>(theWaiter.ResponseJson);
        }
    }
}
