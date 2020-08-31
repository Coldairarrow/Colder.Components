using System.Threading.Tasks;

namespace Colder.MessageBus.Abstractions
{
    /// <summary>
    /// 消息总线接口
    /// </summary>
    public interface IMessageBus
    {
        /// <summary>
        /// 发布事件(广播)
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="message">消息</param>
        /// <returns></returns>
        Task Publish<T>(T message) where T : IMessage;

        /// <summary>
        /// 发送命令(单播)
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="message">消息</param>
        /// <param name="endpointName">指定消费节点</param>
        /// <returns></returns>
        Task Send<T>(T message, string endpointName) where T : IMessage;
    }
}
