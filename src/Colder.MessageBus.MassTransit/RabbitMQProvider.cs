using Colder.MessageBus.Abstractions;
using Colder.MessageBus.InMemory;
using MassTransit;
using System;

namespace Colder.MessageBus.RabbitMQ
{
    internal class RabbitMQProvider : MassTransitProvider
    {
        public RabbitMQProvider(IServiceProvider serviceProvider, MessageBusOptions options) : base(serviceProvider, options)
        {
        }

        protected override IBusControl BuildBusControl()
        {
            return Bus.Factory.CreateUsingRabbitMq(busFactoryBuilder =>
            {
                busFactoryBuilder.Host(Options.Host, config =>
                {
                    if (!string.IsNullOrEmpty(Options.Username) && !string.IsNullOrEmpty(Options.Password))
                    {
                        config.Username(Options.Username);
                        config.Password(Options.Password);
                    }
                });

                ConfigBusFactory(busFactoryBuilder);
                
                busFactoryBuilder.ReceiveEndpoint(Options.Endpoint, endpointBuilder =>
                {
                    //并发数配置
                    if (Options.Concurrency != 0)
                    {
                        endpointBuilder.PrefetchCount = Options.Concurrency;
                    }

                    ConfigEndpoint(endpointBuilder);
                });
            });
        }
    }
}
