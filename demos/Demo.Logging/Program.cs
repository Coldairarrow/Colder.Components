using Colder.Logging.Serilog;
using Colder.MessageBus.Abstractions;
using Colder.MessageBus.MassTransit;
using Demo.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Logging.Demo
{
    class SendEventHostService : BackgroundService
    {
        private readonly IMessageBus _messageBus;
        private readonly ILogger _logger;
        private Timer _timer;
        public SendEventHostService(IMessageBus messageBus, ILogger<SendEventHostService> logger)
        {
            _messageBus = messageBus;
            _logger = logger;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(async state =>
            {
                try
                {
                    await _messageBus.Publish(new TestEvent() { Text = Guid.NewGuid().ToString() });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }, null, 0, 1000);

            return Task.CompletedTask;
        }
    }
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLoggingDefaults()
                .ConfigureMessageBusDefaults(Endpoints.TestPoint)
                .ConfigureServices(services =>
                {
                    services.AddHostedService<SendEventHostService>();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
