namespace Colder.WebSockets.Abstractions
{
    /// <summary>
    /// WebSocket服务端
    /// </summary>
    public interface IWebSocketServer
    {
        /// <summary>
        /// 通过id获取连接
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IWebSocketConnection GetConnection(string id);

        /// <summary>
        /// 获取所有连接
        /// </summary>
        /// <returns></returns>
        IWebSocketConnection[] GetAllConnections();

        /// <summary>
        /// 连接数
        /// </summary>
        public int ConnectionCount { get; }
    }
}
