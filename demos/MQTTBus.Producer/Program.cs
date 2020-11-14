using Colder.Logging.Serilog;
using Colder.MessageBus.Abstractions;
using Colder.MessageBus.Hosting;
using Demo.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace MQTTBus.Producer
{
    class Program
    {
        public static async Task Main()
        {
            await Host.CreateDefaultBuilder()
               .ConfigureLoggingDefaults()
               .ConfigureServices(services =>
               {
                   services.AddHostedService<SendTest>();
                   services.AddMessageBus(new MessageBusOptions
                   {
                       Host = "localhost:1883",
                       Transport = TransportType.MQTT,
                       Username = "guest",
                       Password = "guest",
                       Endpoint = MessageBusEndpoints.Producer
                   });
               })
               .RunConsoleAsync();
        }
    }
}
