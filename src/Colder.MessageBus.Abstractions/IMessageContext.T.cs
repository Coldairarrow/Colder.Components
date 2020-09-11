namespace Colder.MessageBus.Abstractions
{
    /// <summary>
    /// 消息上下文
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public interface IMessageContext<TMessage> : IMessageContext where TMessage : class, IMessage
    {
        /// <summary>
        /// 消息内容
        /// </summary>
        TMessage Message { get; }
    }
}
