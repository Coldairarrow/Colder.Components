using Colder.Logging.Serilog;
using Colder.MessageBus.Abstractions;
using Colder.MessageBus.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace Colder.CommonUtil.Tests
{
    [TestClass]
    public class MessageBusTest
    {
        [TestMethod]
        public void Test()
        {
            var host = Host.CreateDefaultBuilder()
               .ConfigureLoggingDefaults()
               .ConfigureServices(services =>
               {
                   services.AddHostedService<SendTest>();
                   services.AddMessageBus(new MessageBusOptions
                   {
                       Host = "localhost",
                       Transport = TransportType.RabbitMQ
                   });
               })
               .Build();
            host.Start();

            Thread.Sleep(3000);
        }
    }
}
