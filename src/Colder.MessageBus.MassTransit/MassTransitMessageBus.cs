using Colder.MessageBus.Abstractions;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace Colder.MessageBus.MassTransit
{
    internal class MassTransitMessageBus : IMessageBus
    {
        private readonly IBusControl _busControl;
        public MassTransitMessageBus(IBusControl busControl)
        {
            _busControl = busControl;
        }
        public async Task Publish<T>(T message) where T : IEvent
        {
            await _busControl.Publish(message);
        }
        public async Task Send<T>(T message, Uri destination) where T : ICommand
        {
            var channel = await _busControl.GetSendEndpoint(destination);

            await channel.Send(message);
        }
        public async Task<TResponse> Request<TRequest, TResponse>(TRequest message, Uri destination, TimeSpan? timeout = null)
            where TRequest : class, ICommand where TResponse : class
        {
            timeout ??= TimeSpan.FromSeconds(30);
            var reqTimeout = RequestTimeout.After(0, 0, 0, 0, (int)timeout.Value.TotalMilliseconds);
            var response = await _busControl.Request<TRequest, TResponse>(destination, message, default, reqTimeout);

            return response.Message;
        }
    }
}
