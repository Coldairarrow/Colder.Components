using Colder.MessageBus.Abstractions;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Colder.MessageBus.MassTransit
{
    internal class MassTransitMessageBusHostService : BackgroundService
    {
        private readonly IMessageBus _messageBus;
        public MassTransitMessageBusHostService(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}
