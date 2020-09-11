using Colder.MessageBus.Abstractions;
using MassTransit;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Colder.MessageBus.MassTransit
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

        public async Task Publish<T>(T message) where T : IEvent
        {
            await _busControl.Publish(message, GetCancellationToken());
        }
        public async Task Send<T>(T message, Uri destination) where T : ICommand
        {
            var channel = await _busControl.GetSendEndpoint(destination);

            await channel.Send(message, GetCancellationToken());
        }
        public async Task<TResponse> Request<TRequest, TResponse>(TRequest message, Uri destination)
            where TRequest : class, ICommand where TResponse : class
        {
            var reqTimeout = RequestTimeout.After(0, 0, 0, _options.SendMessageTimeout);
            var response = await _busControl.Request<TRequest, TResponse>(destination, message, default, reqTimeout);

            return response.Message;
        }
    }
}
