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



            var provider = services.BuildServiceProvider();

            var busControl = provider.GetRequiredService<IBusControl>();

            await busControl.StartAsync();

            Console.ReadLine();
        }
    }
}
