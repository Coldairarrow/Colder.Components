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
                ConfigBusFactory(busFactoryBuilder);

                busFactoryBuilder.Host(Options.Host, config =>
                {
                    if (!string.IsNullOrEmpty(Options.Username) && !string.IsNullOrEmpty(Options.Password))
                    {
                        config.Username(Options.Username);
                        config.Password(Options.Password);
                    }
                });
            });
        }
    }
}
