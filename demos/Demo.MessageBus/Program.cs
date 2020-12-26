using Colder.Logging.Serilog;
using Colder.MessageBus.Hosting;
using Microsoft.Extensions.DependencyInjection;
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
                .ConfigureMessageBusDefaults()
                .ConfigureServices(services =>
                {
                    services.AddHostedService<PublishService>();
                })
                .RunConsoleAsync();
        }
    }
}
