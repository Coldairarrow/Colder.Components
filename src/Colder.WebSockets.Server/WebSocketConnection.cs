using Colder.WebSockets.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Colder.WebSockets.Server
{
    internal class WebSocketConnection : IWebSocketConnection, IDisposable
    {
        private readonly WebSocketServerOptions _webSocketServerOption;
        private readonly IServiceProvider _serviceProvider;
        private readonly WebSocketServer _webSocketServer;
        public WebSocketConnection(WebSocketServer webSocketServer, WebSocket webSocket, string id, IServiceProvider serviceProvider)
        {
            _webSocketServer = webSocketServer;
            WebSocket = webSocket;
            Id = id;
            _serviceProvider = serviceProvider;
            _webSocketServerOption = serviceProvider.GetService<IOptions<WebSocketServerOptions>>().Value;
        }
        public readonly WebSocket WebSocket;
        public string Id { get; set; }

        public async Task Close()
        {
            await WebSocket.CloseAsync(WebSocketCloseStatus.Empty, string.Empty, CancellationToken.None);
            WebSocket.Dispose();
            _webSocketServer.RemoveConnection(this);
        }

        public async Task Send(string msg)
        {
            await WebSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(msg)), WebSocketMessageType.Text, true, CancellationToken.None);
            if (_webSocketServerOption.OnSend != null)
            {
                await _webSocketServerOption.OnSend(_serviceProvider, this, msg);
            }
        }

        public void Dispose()
        {
            ((IDisposable)WebSocket).Dispose();
        }
    }
}
