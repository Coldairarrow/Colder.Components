namespace Colder.Cache
{
    /// <summary>
    /// 缓存配置
    /// </summary>
    public class CacheOptions
    {
        /// <summary>
        /// 缓存类型
        /// </summary>
        public CacheTypes CacheType { get; set; }

        /// <summary>
        /// Redis连接字符串
        /// 配置参考 https://stackexchange.github.io/StackExchange.Redis/Configuration.html
        /// </summary>
        public string RedisConnectionString { get; set; }
    }
}
