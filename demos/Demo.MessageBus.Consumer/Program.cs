using Demo.Common;
using GreenPipes;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Demo.MessageBus.Consumer
{
    class ProxyHandler
    {
        private readonly IServiceProvider _serviceProvider;
        public ProxyHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task Handle<T>(ConsumeContext<T> context) where T : class
        {
            var logger = _serviceProvider.GetService<ILogger<ProxyHandler>>();

            logger.LogInformation(context.MessageId.ToString());
            return Task.CompletedTask;

        }
    }
    class Program
    {
        public static Task Handle<T>(ConsumeContext<T> context) where T : class
        {
            Console.WriteLine(context.MessageId);
            return Task.CompletedTask;
        }

        public static async Task Main()
        {
            var services = new ServiceCollection();
            services.AddLogging(config =>
            {
                config.AddConsole(x => x.TimestampFormat = "HH:mm:ss.fff");
            });

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.UseRetry(retryCfg =>
                    {
                        retryCfg.Interval(3, TimeSpan.FromSeconds(1));
                    });

                    //MessageHandler<TestEvent> message = Hanlde;
                    cfg.ReceiveEndpoint(Endpoints.TestPoint, ep =>
                    {
                        var delegateType = typeof(MessageHandler<>).MakeGenericType(typeof(TestEvent));
                        var bindMethod = typeof(ProxyHandler)
                            .GetMethod("Handle")
                            .MakeGenericMethod(typeof(TestEvent));
                        ProxyHandler proxyHandler = new ProxyHandler(context);
                        var theDelegate = Delegate.CreateDelegate(delegateType, proxyHandler, bindMethod);
                        var method = typeof(HandlerExtensions).GetMethod("Handler")
                            .MakeGenericMethod(typeof(TestEvent));
                        method.Invoke(null, new object[] { ep, theDelegate, null });
                        //HandlerExtensions.Handler<TestEvent>(ep, message);
                        //ep.Handler<TestEvent>(async context =>
                        //{
                        //    Console.WriteLine($"Received: {context.Message.Text}");

                        //    await Task.CompletedTask;
                        //});
                    });
                });
            });

            var provider = services.BuildServiceProvider();

            var busControl = provider.GetRequiredService<IBusControl>();

            await busControl.StartAsync();

            Console.ReadLine();
        }
    }
}
