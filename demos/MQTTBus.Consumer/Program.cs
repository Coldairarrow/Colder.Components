using Colder.Logging.Serilog;
using Colder.MessageBus.Abstractions;
using Colder.MessageBus.Hosting;
using Demo.Common;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace MQTTBus.Consumer
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
                       Host = "localhost:1883",
                       Transport = TransportType.MQTT,
                       Username = "guest",
                       Password = "guest",
                       Endpoint = MessageBusEndpoints.Consumer
                   });
               })
               .RunConsoleAsync();
        }
    }
}