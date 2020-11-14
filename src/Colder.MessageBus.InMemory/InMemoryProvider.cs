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
            });
        }
    }
}
