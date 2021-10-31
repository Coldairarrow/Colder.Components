using Colder.Common;
using Colder.OpenService.Client;
using Demo.OpenService.Common;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Demo.OpenService.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddOpenServiceClient(typeof(IHelloOpenService).Assembly, new OpenServiceOptions
            {
                BaseUrl = "http://localhost:5000/api/"
            });
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            IHelloOpenService helloOpenService = serviceProvider.GetService<IHelloOpenService>();
            var response = await helloOpenService.SayHello(new IdInput<string> { Id = "Hello World" });
            Console.WriteLine($"请求成功 返回参:{response}");

            Console.ReadLine();
        }
    }
}
