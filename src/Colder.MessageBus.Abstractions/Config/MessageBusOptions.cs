namespace Colder.MessageBus.Abstractions
{
    /// <summary>
    /// 消息总线配置
    /// </summary>
    public class MessageBusOptions
    {
        /// <summary>
        /// 传输介质
        /// </summary>
        public TransportType Transport { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
    }
}
