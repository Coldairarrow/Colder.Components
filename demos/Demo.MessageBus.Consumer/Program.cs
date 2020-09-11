using Colder.MessageBus.Abstractions;
using Colder.MessageBus.MassTransit;
using Demo.Common;
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
            }, Endpoints.TestPoint);

            var provider = services.BuildServiceProvider();
            var messageBus = provider.GetService<IMessageBus>();

            await Task.CompletedTask;

            Console.ReadLine();
        }
    }
}
