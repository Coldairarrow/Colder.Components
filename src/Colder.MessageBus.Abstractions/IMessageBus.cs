using System.Threading.Tasks;

namespace Colder.MessageBus.Abstractions
{
    /// <summary>
    /// 消息总线接口
    /// </summary>
    public interface IMessageBus
    {
        /// <summary>
        /// 发布事件
        /// </summary>
        /// <typeparam name="TMessage">消息类型</typeparam>
        /// <param name="message">消息</param>
        /// <param name="endpoint">指定消费节点</param>
        /// <returns></returns>
        Task Publish<TMessage>(TMessage message, string endpoint = null) where TMessage : class;

        /// <summary>
        /// 发送请求
        /// </summary>
        /// <typeparam name="TRequest">请求数据类型</typeparam>
        /// <typeparam name="TResponse">返回数据类型</typeparam>
        /// <param name="message">消息</param>
        /// <param name="endpoint">指定消费节点</param>
        /// <returns></returns>
        Task<TResponse> Request<TRequest, TResponse>(TRequest message, string endpoint)
           where TRequest : class
           where TResponse : class;
    }
}
