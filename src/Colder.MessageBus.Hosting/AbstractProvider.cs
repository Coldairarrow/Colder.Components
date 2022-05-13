using Colder.MessageBus.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Colder.MessageBus.Hosting
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class AbstractProvider
    {
        /// <summary>
        /// 
        /// </summary>
        protected readonly IServiceProvider ServiceProvider;

        /// <summary>
        /// 
        /// </summary>
        protected readonly IServiceScopeFactory ServiceScopeFactory;

        /// <summary>
        /// 
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// 
        /// </summary>
        protected readonly ILoggerFactory LoggerFactory;

        /// <summary>
        /// 
        /// </summary>
        protected readonly MessageBusOptions Options;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="options"></param>
        protected AbstractProvider(IServiceProvider serviceProvider, MessageBusOptions options)
        {
            ServiceProvider = serviceProvider;
            ServiceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
            Options = options;
            LoggerFactory = serviceProvider.GetService<ILoggerFactory>();
            Logger = LoggerFactory.CreateLogger(GetType());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract IMessageBus GetBusInstance();
    }
}
