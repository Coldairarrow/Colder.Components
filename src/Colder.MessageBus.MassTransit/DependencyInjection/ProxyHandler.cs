using Colder.MessageBus.Abstractions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using System;
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
            MassTransitMessageContext<T> msgContext = new MassTransitMessageContext<T>();
            msgContext.Message = context.Message;

            var interfaceType = typeof(IMessageHandler<>).MakeGenericType(typeof(T));
            var theHandler = AssemblyHelper.HanlderTypes.Where(x => interfaceType.IsAssignableFrom(x))
                .FirstOrDefault();

            var handler = ActivatorUtilities.CreateInstance(_serviceProvider, theHandler) as IMessageHandler<T>;
            await handler.Handle(msgContext);
        }
    }
}
