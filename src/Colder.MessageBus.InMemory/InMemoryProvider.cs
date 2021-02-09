using Colder.MessageBus.Abstractions;
using MassTransit;
using System;

namespace Colder.MessageBus.InMemory
{
    internal class InMemoryProvider : MassTransitProvider
    {
        public InMemoryProvider(IServiceProvider serviceProvider, MessageBusOptions options) : base(serviceProvider, options)
        {
        }

        protected override IBusControl BuildBusControl()
        {
            return Bus.Factory.CreateUsingInMemory(busFactoryBuilder =>
            {
                ConfigBusFactory(busFactoryBuilder);

                busFactoryBuilder.ReceiveEndpoint(Options.Endpoint, endpointBuilder =>
                {
                    //并发数配置
                    if (Options.Concurrency != 0)
                    {
                        endpointBuilder.ConcurrencyLimit = Options.Concurrency;
                    }

                    ConfigEndpoint(endpointBuilder);
                });
            });
        }
    }
}
