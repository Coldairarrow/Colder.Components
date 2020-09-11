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
        public TransportType Transport { get; set; } = TransportType.InMemory;

        /// <summary>
        /// 地址
        /// </summary>
        public string Host { get; set; } = "localhost";

        /// <summary>
        /// 虚拟主机
        /// </summary>
        public string VirtualHost { get; set; } = "/";

        /// <summary>
        /// 端口
        /// </summary>
        public ushort Port { get; set; } = 5672;

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; } = "guest";

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = "guest";

        /// <summary>
        /// 发送消息超时时间,单位(秒)
        /// </summary>
        public int SendMessageTimeout { get; set; } = 30;
    }
}
