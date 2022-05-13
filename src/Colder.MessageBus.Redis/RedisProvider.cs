using Colder.Common;
using Colder.MessageBus.Abstractions;
using Colder.MessageBus.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colder.MessageBus.Redis
{
    internal class RedisProvider : AbstractProvider
    {
        private static readonly List<object> _objs = new List<object>();
        public static ConcurrentDictionary<Guid, TaskCompletionSource<string>> RequestCallBack
            = new ConcurrentDictionary<Guid, TaskCompletionSource<string>>();
        public RedisProvider(IServiceProvider serviceProvider, MessageBusOptions options)
            : base(serviceProvider, options)
        {
        }

        public override IMessageBus GetBusInstance()
        {
            var clientId = Guid.NewGuid().ToString();
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(Options.Host);
            _objs.Add(redis);
            BlockingCollection<(string topic, string payload)> queue = new BlockingCollection<(string topic, string payload)>();
            _objs.Add(queue);
            for (var i = 0; i < Options.Concurrency; i++)
            {
                _objs.Add(Task.Factory.StartNew(async () =>
                {
                    foreach (var message in queue.GetConsumingEnumerable())
                    {
                        try
                        {
                            await HandleReceive(message.topic, message.payload, clientId, redis);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex, ex.Message);
                        }
                    }
                }, TaskCreationOptions.LongRunning));
            }

            //Topic格式
            //RootTopic/{SourceClientId}/{TargetClientId}/{SourceEndpoint}/{TargetEndpoint}/{MessageBodyType}/{MessageType}/{MessageId}
            string topic;
            foreach (var aMessageType in Cache.MessageTypes)
            {
                //事件广播
                topic = $"{Topic.RootTopic}/*/*/*/*/{aMessageType.FullName}/{MessageTypes.Event}/*";
                SubscribeTopic(topic);

                //命令单播
                topic = $"{Topic.RootTopic}/*/*/*/{Options.Endpoint}/{aMessageType.FullName}/{MessageTypes.Command}/+";
                SubscribeTopic(topic);
            }

            //请求返回
            topic = $"{Topic.RootTopic}/*/{clientId}/*/*/*/{MessageTypes.Response}/*";
            SubscribeTopic(topic);

            Logger.LogInformation("MessageBus:Started (Host:{Host})", Options.Host);

            return new RedisMessageBus(Options, redis, clientId);

            void SubscribeTopic(string topic)
            {
                var channel = redis.GetSubscriber().Subscribe(topic);
                _objs.Add(channel);
                Logger.LogInformation("MessageBus:Subscribe To Topic {Topic}", topic);
                channel.OnMessage(message =>
                {
                    queue.Add((message.Channel, message.Message));
                });
            }
        }

        public async Task HandleReceive(string topicString, string payload, string clientId, ConnectionMultiplexer redis)
        {
            Topic topic = Topic.Parse(topicString);

            //请求返回
            if (topic.MessageType == MessageTypes.Response)
            {
                if (RequestCallBack.TryGetValue(topic.MessageId, out TaskCompletionSource<string> callBack))
                {
                    callBack.SetResult(payload);
                }

                return;
            }

            var messageType = Cache.MessageTypes.Where(x => x.FullName == topic.MessageBodyType).FirstOrDefault();
            if (messageType == null)
            {
                return;
            }

            if (Cache.Message2Handler.TryGetValue(messageType, out Type theHandlerType))
            {
                using var scop = ServiceProvider.CreateScope();

                var messageContextType = typeof(MessageContext<>).MakeGenericType(messageType);
                var messageContext = Activator.CreateInstance(messageContextType) as MessageContext;
                object message = JsonConvert.DeserializeObject(payload, messageType);

                messageContext.ServiceProvider = scop.ServiceProvider;
                messageContext.MessageId = topic.MessageId;
                messageContext.MessageBody = payload;
                messageContext.SetPropertyValue("Message", message);

                var handlerInstance = ActivatorUtilities.CreateInstance(scop.ServiceProvider, theHandlerType);

                var method = theHandlerType.GetMethod("Handle", new Type[] { messageContextType });

                var task = method.Invoke(handlerInstance, new object[] { messageContext }) as Task;
                await task;

                //请求返回
                if (messageContext.Response != null)
                {
                    Topic responseTopic = new Topic
                    {
                        MessageId = topic.MessageId,
                        MessageBodyType = messageContext.Response.GetType().FullName,
                        MessageType = MessageTypes.Response,
                        SourceClientId = clientId,
                        SourceEndpoint = Options.Endpoint,
                        TargetClientId = topic.SourceClientId,
                        TargetEndpoint = topic.SourceEndpoint
                    };

                    await redis.GetSubscriber().PublishAsync(responseTopic.ToString(), JsonConvert.SerializeObject(messageContext.Response));
                }
            }
        }
    }
}
