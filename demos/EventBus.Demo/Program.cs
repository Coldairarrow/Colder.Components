using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventBus.Demo
{
    public class Message
    {
        public string Text { get; set; }
    }
    class Program
    {
        public static async Task Main()
        {
            var services = new ServiceCollection();

            services.AddMassTransit(x =>
            {
                //x.AddConsumer<SubmitOrderConsumer>(typeof(SubmitOrderConsumerDefinition));

                //x.SetKebabCaseEndpointNameFormatter();

                x.UsingInMemory((context, cfg) =>
                {
                    cfg.ReceiveEndpoint("test_queue", ep =>
                    {
                        ep.Handler<Message>(context =>
                        {
                            Console.WriteLine($"接收时间:{DateTime.Now}");
                            return Console.Out.WriteLineAsync($"Received: {context.Message.Text}");
                        });
                    });
                });
            });

            var provider = services.BuildServiceProvider();

            var busControl = provider.GetRequiredService<IBusControl>();

            await busControl.StartAsync(new CancellationTokenSource(TimeSpan.FromSeconds(10)).Token);
            while (true)
            {
                Console.WriteLine($"发送时间:{DateTime.Now}");
                await busControl.Publish(new Message { Text = $"{DateTime.Now}Hi" });
                await Task.Delay(500);
            }
            try
            {
                Console.WriteLine("Press enter to exit");

                await Task.Run(() => Console.ReadLine());
            }
            finally
            {
                await busControl.StopAsync();
            }
        }
    }
}
