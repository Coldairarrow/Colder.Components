using Colder.MessageBus.Abstractions;
using MassTransit;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Colder.MessageBus.InMemory
{
    internal class MassTransitMessageBus : IMessageBus
    {
        private readonly IBusControl _busControl;
        private readonly MessageBusOptions _options;
        public MassTransitMessageBus(IBusControl busControl, MessageBusOptions options)
        {
            _busControl = busControl;
            _options = options;
        }
        private CancellationToken GetCancellationToken()
        {
            return new CancellationTokenSource(TimeSpan.FromSeconds(_options.SendMessageTimeout)).Token;
        }
        private Uri BuildUri(string endpoint)
        {
            return new Uri($"{_options.Host}{endpoint}");
        }

        public async Task Publish<TMessage>(TMessage message, string endpoint) where TMessage : class, IMessage
        {
            if (string.IsNullOrEmpty(endpoint))
            {
                await _busControl.Publish(message, GetCancellationToken());
            }
            else
            {
                var channel = await _busControl.GetSendEndpoint(BuildUri(endpoint));

                await channel.Send(message, GetCancellationToken());
            }
        }
        public async Task<TResponse> Request<TRequest, TResponse>(TRequest message, string endpoint)
            where TRequest : class, IMessage
           where TResponse : class
        {
            var reqTimeout = RequestTimeout.After(0, 0, 0, _options.SendMessageTimeout);
            var response = await _busControl.Request<TRequest, TResponse>(BuildUri(endpoint), message, default, reqTimeout);

            return response.Message;
        }
    }
}
