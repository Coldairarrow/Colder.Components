namespace Colder.Logging.Abstractions
{
    /// <summary>
    /// Kafka配置
    /// </summary>
    public class KafkaOption : EnableOption
    {
        /// <summary>
        /// 服务器地址
        /// </summary>
        public string Brokers { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Topic
        /// </summary>
        public string Topic { get; set; } = "log";
    }
}
