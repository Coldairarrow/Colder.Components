using Castle.DynamicProxy;
using Colder.MessageBus.Abstractions;
using Dynamitey;
using GreenPipes;
using MassTransit;
using MassTransit.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Colder.MessageBus.MassTransit
{
    /// <summary>
    /// 消息总线拓展
    /// </summary>
    public static class DependencyInjectionExtentions
    {
        #region 私有成员

        private static readonly ProxyGenerator Generator = new ProxyGenerator();
        private class ActLikeInterceptor : IInterceptor
        {
            public ActLikeInterceptor(object obj)
            {
                _obj = obj;
            }
            private object _obj;
            public void Intercept(IInvocation invocation)
            {
                var name = new InvokeMemberName(invocation.Method.Name, invocation.Method.GetGenericArguments());

                if (invocation.Method.ReturnType != typeof(void))
                {
                    invocation.ReturnValue = Dynamic.InvokeMember(_obj, name, invocation.Arguments);
                }
                else
                {
                    Dynamic.InvokeMemberAction(_obj, name, invocation.Arguments);
                }
            }
        }
        private static MassTransitMessageBus GetBusInstance(IServiceProvider serviceProvider, MessageBusOptions options, string endpoint = Constant.SENDONLY)
        {
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(MethodBase.GetCurrentMethod().DeclaringType);

            LogContext.ConfigureCurrentLogContext(loggerFactory);

            IBusControl busControl = null;
            logger.LogInformation("MessageBus:Use {TransportType} Transport", options.Transport);
            if (options.Transport == TransportType.InMemory)
            {
                busControl = Bus.Factory.CreateUsingInMemory(busFactoryBuilder =>
                {
                    ConfigBusFactory(busFactoryBuilder);
                    busFactoryBuilder.ReceiveEndpoint(endpoint, endpointBuilder =>
                    {
                        ConfigEndpoint(endpointBuilder);
                    });
                });
            }
            else if (options.Transport == TransportType.RabbitMQ)
            {
                busControl = Bus.Factory.CreateUsingRabbitMq(busFactoryBuilder =>
                {
                    ConfigBusFactory(busFactoryBuilder);

                    busFactoryBuilder.Host(options.Host, config =>
                    {
                        if (!string.IsNullOrEmpty(options.Username) && !string.IsNullOrEmpty(options.Password))
                        {
                            config.Username(options.Username);
                            config.Password(options.Password);
                        }
                    });

                    busFactoryBuilder.ReceiveEndpoint(endpoint, endpointBuilder =>
                    {
                        ConfigEndpoint(endpointBuilder);
                    });
                });
            }
            else
            {
                throw new Exception($"Transport:{options.Transport}无效");
            }

            void ConfigBusFactory(IBusFactoryConfigurator busFactoryConfigurator)
            {
                busFactoryConfigurator.UseRetry(retryCfg =>
                {
                    retryCfg.Interval(3, TimeSpan.FromMilliseconds(100));
                });
            }

            void ConfigEndpoint(IReceiveEndpointConfigurator receiveEndpointConfigurator)
            {
                //绑定消费者
                if (endpoint != Constant.SENDONLY)
                {
                    Cache.MessageTypes.ForEach(messageType =>
                    {
                        logger.LogInformation("MessageBus:Subscribe {MessageType}", messageType);
                        var delegateType = typeof(MessageHandler<>).MakeGenericType(messageType);
                        var bindMethod = typeof(ProxyHandler)
                            .GetMethod("Handle")
                            .MakeGenericMethod(messageType);
                        ProxyHandler proxyHandler = new ProxyHandler(serviceProvider);
                        var theDelegate = Delegate.CreateDelegate(delegateType, proxyHandler, bindMethod);
                        var method = typeof(HandlerExtensions).GetMethod("Handler")
                            .MakeGenericMethod(messageType);
                        method.Invoke(null, new object[] { receiveEndpointConfigurator, theDelegate, null });
                    });
                }

                //异常处理
                receiveEndpointConfigurator.ConfigureError(error =>
                {
                    error.UseInlineFilter((context, next) =>
                    {
                        string body = Encoding.UTF8.GetString(context.GetBody());
                        logger.LogError(context.Exception, "Handle Message Error,MessageBody:{MessageBody}", body);

                        return Task.CompletedTask;
                    });
                });
            }

            busControl.Start();
            logger.LogInformation($"MessageBus:Started");

            return new MassTransitMessageBus(busControl, options);
        }

        #endregion

        #region 外部接口

        /// <summary>
        /// 配置消息总线
        /// </summary>
        /// <param name="hostBuilder">宿主构造</param>
        /// <param name="endpoint">节点</param>
        /// <returns></returns>
        public static IHostBuilder ConfigureMessageBusDefaults(this IHostBuilder hostBuilder, string endpoint = Constant.SENDONLY)
        {
            hostBuilder.ConfigureServices((host, services) =>
            {
                var configuration = host.Configuration;
                var options = configuration.GetSection("messagebus").Get<MessageBusOptions>();
                if (options == null)
                {
                    throw new Exception("消息总线初始化异常：缺少配置信息");
                }

                services.AddMessageBus<IMessageBus>(options, endpoint);
            });

            return hostBuilder;
        }

        /// <summary>
        /// 注入消息总线
        /// </summary>
        /// <param name="services">服务容器</param>
        /// <param name="endpoint">节点</param>
        /// <param name="options">配置</param>
        /// <returns></returns>
        public static IServiceCollection AddMessageBus(this IServiceCollection services, MessageBusOptions options, string endpoint = Constant.SENDONLY)
        {
            return AddMessageBus<IMessageBus>(services, options, endpoint);
        }

        /// <summary>
        /// 注入自定义消息总线
        /// 注：可以通过此方法注入多个总线
        /// </summary>
        /// <typeparam name="TMessageBus">自定义总线类型</typeparam>
        /// <param name="services">服务容器</param>
        /// <param name="endpoint">节点</param>
        /// <param name="options">配置</param>
        /// <returns></returns>
        public static IServiceCollection AddMessageBus<TMessageBus>(this IServiceCollection services, MessageBusOptions options, string endpoint = Constant.SENDONLY)
            where TMessageBus : class, IMessageBus
        {
            Cache.BusTypes.Add(typeof(TMessageBus));
            services.AddHostedService<Bootstraper>();

            return services.AddSingleton(_ =>
            {
                var busInstance = GetBusInstance(_, options, endpoint);

                if (typeof(TMessageBus) == typeof(IMessageBus))
                    return (IMessageBus)busInstance;
                else
                    return Generator.CreateInterfaceProxyWithoutTarget<TMessageBus>(new ActLikeInterceptor(busInstance));
            });
        }

        #endregion
    }
}
