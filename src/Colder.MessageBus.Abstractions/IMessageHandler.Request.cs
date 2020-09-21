using System.Threading.Tasks;

namespace Colder.MessageBus.Abstractions
{
    /// <summary>
    /// 消费者接口
    /// </summary>
    /// <typeparam name="TRequest">请求数据类型</typeparam>
    /// <typeparam name="TResponse">返回数据类型</typeparam>
    public interface IMessageHandler<TRequest,TResponse> where TRequest : class, IMessage
    {
        /// <summary>
        /// 消费消息
        /// </summary>
        /// <param name="context">消息上下文</param>
        /// <returns></returns>
        Task<TResponse> Handle(IMessageContext<TRequest> context);
    }
}
