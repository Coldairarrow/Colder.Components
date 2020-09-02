using Colder.MessageBus.Abstractions;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Colder.MessageBus.MassTransit
{
    public static class MassTransitMessageBusExtentions
    {
        public static IHostBuilder ConfigureMessageBusDefaults(this IHostBuilder hostBuilder, string endpoint)
        {
            hostBuilder.ConfigureServices((host, services) =>
            {
                var configuration = host.Configuration;
                var options = configuration.GetSection("messagebus").Get<MessageBusOptions>();
                if (options == null)
                {
                    throw new Exception("消息总线初始化异常：缺少配置信息");
                }

                IBusControl busControl = null;
                switch (options.Transport)
                {
                    case TransportType.InMemory:
                        {
                            busControl = Bus.Factory.CreateUsingInMemory(sbc =>
                            {
                                sbc.ReceiveEndpoint(endpoint, ep =>
                                {
                                    BindHandler(ep);
                                });
                            });
                        }
                        break;
                    case TransportType.RabbitMQ:
                        {
                            busControl = Bus.Factory.CreateUsingRabbitMq(sbc =>
                            {
                                sbc.ReceiveEndpoint(endpoint, ep =>
                                {
                                    BindHandler(ep);
                                });
                            });
                        }; break;
                    default: throw new Exception($"Transport:{options.Transport}无效");
                }

                void BindHandler(IReceiveEndpointConfigurator receiveEndpointConfigurator)
                {

                }

                busControl.Start();

                services.AddSingleton(busControl);
                services.AddSingleton<IMessageBus, MassTransitMessageBus>();
            });

            return hostBuilder;
        }
    }
}
