using Demo.Common;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Demo.MessageBus.Consumer
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
                        ep.Consumer<Handler1>();
                        ep.Consumer<Handler2>();
                        //ep.Handler<TestEvent>(async context =>
                        //{
                        //    Console.WriteLine($"Received: {context.Message.Text}");

                        //    await Task.CompletedTask;
                        //});
                        //ep.Handler<Handler1>();
                    });
                });
            });

            var provider = services.BuildServiceProvider();

            var busControl = provider.GetRequiredService<IBusControl>();

            await busControl.StartAsync();

            Console.ReadLine();
        }
    }
}
