using Colder.WebSockets.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;

namespace Colder.WebSockets.Server
{
    internal class WebSocketServer : IWebSocketServer
    {
        private readonly SynchronizedCollection<WebSocketConnection> _connections = new SynchronizedCollection<WebSocketConnection>();
        private readonly ReaderWriterLockSlim _readerWriterLockSlim = new ReaderWriterLockSlim();
        public IWebSocketConnection[] GetAllConnections()
        {
            try
            {
                _readerWriterLockSlim.EnterReadLock();
                return _connections.ToArray();
            }
            finally
            {
                _readerWriterLockSlim.ExitReadLock();
            }
        }
        public void AddConnection(WebSocketConnection connection)
        {
            try
            {
                _readerWriterLockSlim.EnterWriteLock();
                _connections.Add(connection);
            }
            finally
            {
                _readerWriterLockSlim.ExitWriteLock();
            }
        }
        public void RemoveConnection(WebSocketConnection connection)
        {
            try
            {
                _readerWriterLockSlim.EnterWriteLock();
                _connections.Remove(connection);
            }
            finally
            {
                _readerWriterLockSlim.ExitWriteLock();
            }
        }
        public IWebSocketConnection GetConnection(string id)
        {
            try
            {
                _readerWriterLockSlim.EnterReadLock();
                return _connections.Where(x => x.Id == id).FirstOrDefault();
            }
            finally
            {
                _readerWriterLockSlim.ExitReadLock();
            }
        }
        public IWebSocketConnection GetConnection(WebSocket webSocket)
        {
            try
            {
                _readerWriterLockSlim.EnterReadLock();
                return _connections.Where(x => x.WebSocket == webSocket).FirstOrDefault();
            }
            finally
            {
                _readerWriterLockSlim.ExitReadLock();
            }
        }
        public int ConnectionCount => _connections.Count;
    }
}
