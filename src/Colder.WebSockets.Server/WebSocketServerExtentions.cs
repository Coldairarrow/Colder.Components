using Colder.WebSockets.Abstractions;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace Colder.WebSockets.Server
{
    /// <summary>
    /// WebSocket服务端拓展
    /// </summary>
    public static class WebSocketServerExtentions
    {
        private static IBusControl _busControl;

        /// <summary>
        /// 注入WebSocket服务端
        /// </summary>
        /// <param name="services"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IServiceCollection AddWebSocketServer(this IServiceCollection services, Action<WebSocketServerOptions> builder)
        {
            services.Configure(builder);
            services.AddSingleton<WebSocketServer>();
            services.AddSingleton<IWebSocketServer>(serviceProvider => serviceProvider.GetService<WebSocketServer>());

            return services;
        }

        /// <summary>
        /// 使用WebSocket服务端
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseWebSocketServer(this IApplicationBuilder app)
        {
            IServiceProvider serviceProvider = app.ApplicationServices;
            IHostApplicationLifetime hostApplicationLifetime = serviceProvider.GetService<IHostApplicationLifetime>();
            WebSocketServerOptions option = serviceProvider.GetService<IOptions<WebSocketServerOptions>>().Value;
            ILogger logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger(typeof(WebSocketServerExtentions));
            WebSocketServer webSocketServer = serviceProvider.GetService<WebSocketServer>();

            app.UseWebSockets();

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            hostApplicationLifetime.ApplicationStopping.Register(() => cancellationTokenSource.Cancel());
            app.Use(async (context, next) =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    WebSocketConnection connection = new WebSocketConnection(
                        webSocketServer, webSocket, Guid.NewGuid().ToString(), serviceProvider);
                    webSocketServer.AddConnection(connection);
                    if (option.OnConnected != null)
                    {
                        await option.OnConnected(serviceProvider, connection);
                    }
                    logger.LogInformation("收到新的连接 当前连接数:{Count}", webSocketServer.ConnectionCount);

                    List<byte> bytes = new List<byte>();
                    try
                    {
                        while (true)
                        {
                            var buffer = new byte[1024];
                            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationTokenSource.Token);
                            if (result.CloseStatus.HasValue)
                            {
                                await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);

                                break;
                            }

                            bytes.AddRange(new ArraySegment<byte>(buffer, 0, result.Count));
                            if (result.EndOfMessage)
                            {
                                //接收数据结束
                                var body = Encoding.UTF8.GetString(bytes.ToArray());

                                //使用内部总线转发，异步处理
                                await _busControl.Publish(new MessageReceivedEvent
                                {
                                    ConnectionId = connection.Id,
                                    Body = body
                                });
                                bytes.Clear();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex is WebSocketException webSocketException && webSocketException.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
                        {
                            //忽略
                        }
                        else
                        {
                            logger.LogError(ex, ex.Message);
                        }
                    }
                    finally
                    {
                        webSocketServer.RemoveConnection(connection);
                        logger.LogInformation("连接关闭[{ConnectionId}] 当前连接数:{Count}", connection.Id, webSocketServer.ConnectionCount);
                    }
                }
                else
                {
                    await next();
                }
            });

            _busControl = Bus.Factory.CreateUsingInMemory(sbc =>
            {
                sbc.ReceiveEndpoint(ep =>
                {
                    ep.Handler<MessageReceivedEvent>(async context =>
                    {
                        var message = context.Message;
                        try
                        {
                            if (option.OnReceive != null)
                            {
                                await option.OnReceive(serviceProvider, webSocketServer.GetConnection(message.ConnectionId), message.Body);
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "处理数据异常 ConnectionId:{ConnectionId} Body:{Body}", message.ConnectionId, message.Body);
                        }
                    });
                });
            });
            _busControl.Start();

            return app;
        }
    }
}
