using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Colder.MessageBus.Hosting
{
    internal class Bootstraper : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        public Bootstraper(IServiceProvider serviceProvider)
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
