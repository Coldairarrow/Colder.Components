using Colder.MessageBus.Abstractions;
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
    public static class MassTransitMessageBusExtentions
    {
        /// <summary>
        /// 配置消息总线
        /// </summary>
        /// <param name="hostBuilder">宿主构造</param>
        /// <param name="endpoint">节点</param>
        /// <returns></returns>
        public static IHostBuilder ConfigureMessageBusDefaults(this IHostBuilder hostBuilder, string endpoint)
        {
            hostBuilder.ConfigureServices((host, services) =>
            {
                services.AddSingleton(serviceProvider =>
                {
                    var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
                    var logger = loggerFactory.CreateLogger(MethodBase.GetCurrentMethod().DeclaringType);

                    LogContext.ConfigureCurrentLogContext(loggerFactory);

                    var configuration = host.Configuration;
                    var options = configuration.GetSection("messagebus").Get<MessageBusOptions>();
                    if (options == null)
                    {
                        throw new Exception("消息总线初始化异常：缺少配置信息");
                    }

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

                            busFactoryBuilder.Host(options.Host, options.Port, options.VirtualHost, config =>
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
                        AssemblyHelper.MessageTypes.ForEach(messageType =>
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

                    return busControl;
                });
                services.AddSingleton<IMessageBus, MassTransitMessageBus>();
                services.AddHostedService<MassTransitMessageBusHostService>();
            });

            return hostBuilder;
        }
    }
}
