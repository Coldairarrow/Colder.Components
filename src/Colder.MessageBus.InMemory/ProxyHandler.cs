using Colder.MessageBus.Abstractions;
using Colder.MessageBus.Hosting;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colder.MessageBus.InMemory
{
    class ProxyHandler
    {
        private readonly IServiceProvider _serviceProvider;
        public ProxyHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Handle<T>(ConsumeContext<T> context) where T : class
        {
            using var scop = _serviceProvider.CreateScope();

            MessageContext<T> msgContext = new MessageContext<T>
            {
                ServiceProvider = _serviceProvider,
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

            var theHandlerType = Cache.Message2Handler[typeof(T)];
            var handlerInstance = ActivatorUtilities.CreateInstance(scop.ServiceProvider, theHandlerType) as IMessageHandler<T>;
            await handlerInstance.Handle(msgContext);

            if (msgContext.Response != null)
            {
                await context.RespondAsync(msgContext.Response);
            }
        }
    }
}
