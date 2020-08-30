using Demo.Common;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Demo.EventBus.Consumer
{
    class Program
    {
        public static async Task Main()
        {
            var services = new ServiceCollection();
            services.AddLogging(config =>
            {
                config.AddConsole(x => x.TimestampFormat = "HH:mm:ss.fff");
            });

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.UseRetry(retryCfg =>
                    {
                        retryCfg.Interval(3, TimeSpan.FromSeconds(1));
                    });

                    cfg.ReceiveEndpoint(Endpoints.TestPoint, ep =>
                    {
                        ep.Handler<ITestEvent>(async context =>
                        {
                            Console.WriteLine($"Received: {context.Message.Text}");

                            await Task.CompletedTask;
                            //throw new Exception("11");
                        });
                    });
                });
            });

            var provider = services.BuildServiceProvider();

            var busControl = provider.GetRequiredService<IBusControl>();

            await busControl.StartAsync(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token);

            Console.ReadLine();
        }
    }
}
