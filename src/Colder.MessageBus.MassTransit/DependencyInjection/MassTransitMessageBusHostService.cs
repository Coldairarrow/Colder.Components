using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Colder.MessageBus.MassTransit
{
    internal class MassTransitMessageBusHostService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        public MassTransitMessageBusHostService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Cache.BusTypes.ForEach(aBusType => _serviceProvider.GetService(aBusType));

            return Task.CompletedTask;
        }
    }
}
