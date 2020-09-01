using Demo.Common;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.MessageBus.Producer
{
    class Program
    {
        public static async Task Main()
        {
            var services = new ServiceCollection();

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });
                });
            });

            var provider = services.BuildServiceProvider();

            var busControl = provider.GetRequiredService<IBusControl>();
            await busControl.StartAsync(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token);

            await busControl.Send(new TestSubEvent { Text = $"{DateTime.Now}Hi" });

            Console.WriteLine($"已发送 {nameof(TestSubEvent)} 事件");

            Console.ReadLine();
        }
    }
}
