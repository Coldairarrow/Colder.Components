using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Orleans
{
    class Bootstrapper : BackgroundService
    {
        private readonly IGrainFactory _grainFactory;
        private Timer _timer;
        private readonly ILogger _logger;
        public Bootstrapper(IGrainFactory grainFactory, ILogger<Bootstrapper> logger)
        {
            _grainFactory = grainFactory;
            _logger = logger;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Guid id = Guid.NewGuid();
            _timer = new Timer(async _ =>
            {
                try
                {
                    await _grainFactory.GetGrain<IHello>(0).Say("小明");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }, null, 0, 1000);

            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _timer.DisposeAsync();

            await base.StopAsync(cancellationToken);
        }
    }
}
