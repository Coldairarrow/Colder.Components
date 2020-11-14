using Colder.MessageBus.Abstractions;
using Colder.MessageBus.Hosting;
using GreenPipes;
using MassTransit;
using MassTransit.Context;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Colder.MessageBus.InMemory
{
    internal abstract class MassTransitProvider : AbstractProvider
    {
        protected MassTransitProvider(IServiceProvider serviceProvider, MessageBusOptions options) 
            : base(serviceProvider, options)
        {
        }

        public override IMessageBus GetBusInstance()
        {
            LogContext.ConfigureCurrentLogContext(LoggerFactory);
            Logger.LogInformation("MessageBus:Use {TransportType} Transport", Options.Transport);
            IBusControl busControl = BuildBusControl();

            busControl.Start();
            Logger.LogInformation($"MessageBus:Started");

            return new MassTransitMessageBus(busControl, Options);
        }
        protected abstract IBusControl BuildBusControl();
        protected void ConfigBusFactory(IBusFactoryConfigurator busFactoryConfigurator)
        {
            busFactoryConfigurator.UseRetry(retryCfg =>
            {
                retryCfg.Interval(3, TimeSpan.FromMilliseconds(1000));
            });

            busFactoryConfigurator.ReceiveEndpoint(Options.Endpoint, endpointBuilder =>
            {
                //绑定消费者
                if (Options.Endpoint != Constant.SENDONLY)
                {
                    Cache.MessageTypes.ForEach(messageType =>
                    {
                        Logger.LogInformation("MessageBus:Subscribe {MessageType}", messageType);
                        var delegateType = typeof(MessageHandler<>).MakeGenericType(messageType);
                        var bindMethod = typeof(ProxyHandler)
                            .GetMethod("Handle")
                            .MakeGenericMethod(messageType);
                        ProxyHandler proxyHandler = new ProxyHandler(ServiceProvider);
                        var theDelegate = Delegate.CreateDelegate(delegateType, proxyHandler, bindMethod);
                        var method = typeof(HandlerExtensions).GetMethod("Handler")
                            .MakeGenericMethod(messageType);
                        method.Invoke(null, new object[] { endpointBuilder, theDelegate, null });
                    });
                }

                //异常处理
                endpointBuilder.ConfigureError(error =>
                {
                    error.UseInlineFilter((context, next) =>
                    {
                        string body = Encoding.UTF8.GetString(context.GetBody());
                        Logger.LogError(context.Exception, "Handle Message Error,MessageBody:{MessageBody}", body);

                        return Task.CompletedTask;
                    });
                });
            });
        }
    }
}
