using Colder.Logging.Serilog;
using Colder.MessageBus.Abstractions;
using Colder.MessageBus.Hosting;
using Logging.Demo;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
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
                .ConfigureServices((host,services) =>
                {
                    MessageBusOptions messageBusOptions = host.Configuration.GetSection("messagebus").Get<MessageBusOptions>();
                    messageBusOptions.Endpoint = Constant.SENDONLY;
                    services.AddHostedService<PublishService>();
                    services.AddMessageBus<ISendOnlyMessageBus>(messageBusOptions);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .RunConsoleAsync();
        }
    }
}
