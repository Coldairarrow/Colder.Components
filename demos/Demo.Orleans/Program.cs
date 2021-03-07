using Colder.Logging.Serilog;
using Colder.Orleans.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Hosting;
using System.Threading.Tasks;

namespace Demo.Orleans
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder(args)
                .ConfigureLoggingDefaults()
                .ConfigureOrleansDefaults()
                .ConfigureServices(services =>
                {
                    services.AddHostedService<Bootstrapper>();
                })
                .RunConsoleAsync();
        }
    }
}
