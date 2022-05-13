using Colder.MessageBus.Abstractions;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Colder.MessageBus.Redis
{
    internal class RedisMessageBus : IMessageBus
    {
        private readonly MessageBusOptions _options;
        private readonly ConnectionMultiplexer _redis;
        private readonly string _clientId;
        public RedisMessageBus(MessageBusOptions options, ConnectionMultiplexer redis, string clientId)
        {
            _options = options;
            _redis = redis;
            _clientId = clientId;
        }

        public async Task Publish<TMessage>(TMessage message, string endpoint) where TMessage : class
        {
            Topic topic = new Topic
            {
                MessageId = Guid.NewGuid(),
                MessageBodyType = message.GetType().FullName,
                MessageType = string.IsNullOrEmpty(endpoint) ? MessageTypes.Event : MessageTypes.Command,
                SourceClientId = _clientId,
                SourceEndpoint = _options.Endpoint,
                TargetClientId = "*",
                TargetEndpoint = string.IsNullOrEmpty(endpoint) ? "*" : endpoint,
            };

            await _redis.GetSubscriber().PublishAsync(topic.ToString(), JsonConvert.SerializeObject(message));
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
                SourceClientId = _clientId,
                SourceEndpoint = _options.Endpoint,
                TargetClientId = "*",
                TargetEndpoint = endpoint
            };

            TaskCompletionSource<string> callBack = new TaskCompletionSource<string>();
            var ct = new CancellationTokenSource(TimeSpan.FromSeconds(_options.SendMessageTimeout));
            ct.Token.Register(() => callBack.TrySetCanceled(), useSynchronizationContext: false);
            RedisProvider.RequestCallBack[topic.MessageId] = callBack;

            await _redis.GetSubscriber().PublishAsync(topic.ToString(), JsonConvert.SerializeObject(message));

            var responseJson = await callBack.Task;

            return JsonConvert.DeserializeObject<TResponse>(responseJson);
        }
    }
}
