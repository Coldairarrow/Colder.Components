namespace Colder.MessageBus.Abstractions
{
    /// <summary>
    /// 消息上下文
    /// </summary>
    /// <typeparam name="TMessage">消息类型</typeparam>
    public class MessageContext<TMessage> : MessageContext where TMessage : class
    {
        /// <summary>
        /// 消息
        /// </summary>
        public TMessage Message { get; set; } = null;
    }
}
