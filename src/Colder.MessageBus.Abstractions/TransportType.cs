namespace Colder.MessageBus.Abstractions
{
    /// <summary>
    /// 传输介质类型
    /// </summary>
    public enum TransportType
    {
        /// <summary>
        /// 内存
        /// </summary>
        InMemory = 1,

        /// <summary>
        /// RabbitMQ
        /// </summary>
        RabbitMQ = 2
    }
}
