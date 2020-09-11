using Colder.MessageBus.Abstractions;
using Colder.MessageBus.MassTransit;
using Demo.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.MessageBus.Producer
{
    class Program
    {
        public static async Task Main()
        {
            var services = new ServiceCollection();

            services.AddLogging(x =>
            {
                x.SetMinimumLevel(LogLevel.Trace);
                x.AddConsole(config =>
                {
                    config.TimestampFormat = "[HH:mm:ss.fff]";
                });
            });
            services.AddMessageBus(new MessageBusOptions
            {
                Host = "localhost",
                Transport = TransportType.RabbitMQ
            });

            var provider = services.BuildServiceProvider();
            var loggerFactory = provider.GetService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(MethodBase.GetCurrentMethod().GetType());

            var bus = provider.GetService<IMessageBus>();
            while (true)
            {
                try
                {
                    var token = new CancellationTokenSource(TimeSpan.FromSeconds(3)).Token;
                    await bus.Publish(new TestSubEvent { Text = $"{DateTime.Now}Hi" });
                    logger.LogInformation($"已发送 {nameof(TestSubEvent)} 事件");

                    Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, ex.Message);
                }
            }

            Console.ReadLine();
        }
    }
}
