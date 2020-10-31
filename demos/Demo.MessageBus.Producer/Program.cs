﻿using Colder.Logging.Serilog;
using Colder.MessageBus.Abstractions;
using Colder.MessageBus.MassTransit;
using Demo.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Demo.MessageBus.Producer
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
                       Transport = TransportType.RabbitMQ,
                       Username = "guest",
                       Password = "guest"
                   }, MessageBusEndpoints.Producer);

                   services.AddHostedService<SendTest>();
               })
               .RunConsoleAsync();
        }
    }
}
