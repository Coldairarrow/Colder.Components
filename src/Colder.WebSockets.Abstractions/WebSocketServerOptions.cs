using System;
using System.Threading.Tasks;

namespace Colder.WebSockets.Abstractions
{
    /// <summary>
    /// SocketServer选项
    /// </summary>
    public class WebSocketServerOptions
    {
        /// <summary>
        /// 已收到新连接事件，参数依次为：容器、当前连接、消息
        /// </summary>
        public Func<IServiceProvider, IWebSocketConnection, Task> OnConnected { get; set; }

        /// <summary>
        /// 已收到数据事件，参数依次为：容器、当前连接、消息
        /// </summary>
        public Func<IServiceProvider, IWebSocketConnection, string, Task> OnReceive { get; set; }

        /// <summary>
        /// 已发送数据事件，参数依次为：容器、当前连接、消息
        /// </summary>
        public Func<IServiceProvider, IWebSocketConnection, string, Task> OnSend { get; set; }
    }
}
