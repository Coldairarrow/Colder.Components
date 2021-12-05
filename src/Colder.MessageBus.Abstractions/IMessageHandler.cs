using System.Threading.Tasks;

namespace Colder.MessageBus.Abstractions
{
    /// <summary>
    /// 消费者接口
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public interface IMessageHandler<TMessage> where TMessage : class
    {
        /// <summary>
        /// 消费消息
        /// </summary>
        /// <param name="context">消息上下文</param>
        /// <returns></returns>
        Task Handle(MessageContext<TMessage> context);
    }
}
