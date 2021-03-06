using Colder.Orleans.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
                .ConfigureOrleansDefaults()
                .ConfigureServices(services =>
                {
                    services.AddHostedService<Bootstrapper>();
                })
                .ConfigureLogging(builder =>
                {
                    builder.AddConsole(options =>
                    {
                        options.TimestampFormat = "HH:mm:ss.fff";
                    });
                })
                .RunConsoleAsync();
        }
    }
}
