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
        /// Redis节点
        /// </summary>
        public string[] RedisEndpoints { get; set; }
    }
}
