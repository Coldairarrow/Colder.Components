using Colder.MessageBus.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Colder.MessageBus.Hosting
{
    internal abstract class AbstractProvider
    {
        protected IServiceProvider ServiceProvider;
        protected ILogger Logger;
        protected ILoggerFactory LoggerFactory;
        protected MessageBusOptions Options;
        protected AbstractProvider(IServiceProvider serviceProvider, MessageBusOptions options)
        {
            ServiceProvider = serviceProvider;
            Options = options;
            LoggerFactory = serviceProvider.GetService<ILoggerFactory>();
            Logger = LoggerFactory.CreateLogger(GetType());
        }

        public abstract IMessageBus GetBusInstance();
    }
}
