﻿using Colder.MessageBus.Abstractions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colder.MessageBus.MassTransit
{
    class ProxyHandler
    {
        private readonly IServiceProvider _serviceProvider;
        public ProxyHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Handle<T>(ConsumeContext<T> context) where T : class, IMessage
        {
            using var scop = _serviceProvider.CreateScope();

            MessageContext<T> msgContext = new MessageContext<T>
            {
                Message = context.Message,
                DestinationAddress = context.DestinationAddress,
                FaultAddress = context.FaultAddress,
                Headers = new Dictionary<string, object>(context.Headers.GetAll().ToDictionary(x => x.Key, x => x.Value)),
                MessageId = context.MessageId,
                ResponseAddress = context.ResponseAddress,
                SentTime = context.SentTime,
                SourceAddress = context.SourceAddress,
                SourceMachineName = context.Host.MachineName
            };

            var handlerType = Cache.MessageHandlers[typeof(T)];
            var handler = ActivatorUtilities.CreateInstance(scop.ServiceProvider, handlerType) as IMessageHandler<T>;
            await handler.Handle(msgContext);
        }
    }
}
