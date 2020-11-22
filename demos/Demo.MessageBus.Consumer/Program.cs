using Colder.Logging.Serilog;
using Colder.MessageBus.Abstractions;
using Colder.MessageBus.Hosting;
using Demo.Common;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Demo.MessageBus.Consumer
{
    class Program
    {
        public static async Task Main()
        {
            await Host.CreateDefaultBuilder()
                .ConfigureLoggingDefaults()
                .ConfigureServices(services =>
                {
                    services.AddMessageBus(new MessageBusOptions
                    {
                        Host = "amqp://localhost:5672/",
                        //Host = "localhost:1883",
                        Transport = TransportType.RabbitMQ,
                        Username = "guest",
                        Password = "guest",
                        Endpoint = MessageBusEndpoints.Consumer
                    });
                })
                .RunConsoleAsync();
        }
    }
}
