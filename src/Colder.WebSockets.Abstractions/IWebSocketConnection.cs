using System.Threading.Tasks;

namespace Colder.WebSockets.Abstractions
{
    /// <summary>
    /// WebSocket连接
    /// </summary>
    public interface IWebSocketConnection
    {
        /// <summary>
        /// 唯一Id
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg">消息</param>
        /// <returns></returns>
        Task Send(string msg);

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <returns></returns>
        Task Close();
    }
}
