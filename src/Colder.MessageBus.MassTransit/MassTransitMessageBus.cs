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
        public Task Publish<T>(T message) where T : IEvent
        {
            return _busControl.Publish(message);
        }

        public Task<TResponse> Request<TRequest, TResponse>(TRequest message, string endpointName) where TRequest : ICommand
        {
            throw new NotImplementedException();
        }

        public Task Send<T>(T message, string endpointName) where T : ICommand
        {
            throw new NotImplementedException();
        }
    }
}
