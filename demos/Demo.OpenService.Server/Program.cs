using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Colder.OpenService.Client;
using System.Reflection;

namespace Demo.OpenService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((host, services) =>
                {
                    services.AddOpenServiceClient(Assembly.GetEntryAssembly(), new OpenServiceOptions
                    {
                        BaseUrl="http://localhost:5000/api/"
                    });
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
