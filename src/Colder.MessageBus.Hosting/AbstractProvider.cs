using Colder.MessageBus.Abstractions;
using System;

namespace Colder.MessageBus.Hosting
{
    internal abstract class AbstractProvider
    {
        public abstract IMessageBus GetBusInstance(IServiceProvider serviceProvider, MessageBusOptions options);
    }
}
