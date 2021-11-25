using Colder.WebSockets.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;

namespace Colder.WebSockets.Server
{
    internal class WebSocketServer : IWebSocketServer
    {
        private readonly SynchronizedCollection<WebSocketConnection> _connections = new SynchronizedCollection<WebSocketConnection>();
        public IWebSocketConnection[] GetAllConnections()
        {
            return _connections.ToArray();
        }

        public void AddConnection(WebSocketConnection connection)
        {
            _connections.Add(connection);
        }

        public void RemoveConnection(WebSocketConnection connection)
        {
            _connections.Remove(connection);
        }

        public IWebSocketConnection GetConnection(string id)
        {
            return _connections.Where(x => x.Id == id).FirstOrDefault();
        }

        public IWebSocketConnection GetConnection(WebSocket webSocket)
        {
            return _connections.Where(x => x.WebSocket == webSocket).FirstOrDefault();
        }
    }
}
