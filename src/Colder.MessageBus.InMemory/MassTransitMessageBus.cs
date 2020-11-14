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

        async Task IMessageBus.Publish<TMessage>(TMessage message, string endpoint)
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
        async Task<TResponse> IMessageBus.Request<TRequest, TResponse>(TRequest message, string endpoint)
        {
            var reqTimeout = RequestTimeout.After(0, 0, 0, _options.SendMessageTimeout);
            var response = await _busControl.Request<TRequest, TResponse>(BuildUri(endpoint), message, default, reqTimeout);

            return response.Message;
        }
    }
}
