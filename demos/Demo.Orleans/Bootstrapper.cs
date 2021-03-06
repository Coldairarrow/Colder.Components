using Microsoft.Extensions.Hosting;
using Orleans;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Orleans
{
    class Bootstrapper : BackgroundService
    {
        private readonly IGrainFactory _grainFactory;
        private Timer _timer;
        public Bootstrapper(IGrainFactory grainFactory)
        {
            _grainFactory = grainFactory;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(async _ =>
            {
                await _grainFactory.GetGrain<IHello>(0).Say("小明");
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
