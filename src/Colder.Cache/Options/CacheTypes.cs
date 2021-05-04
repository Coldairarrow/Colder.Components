namespace Colder.Cache
{
    /// <summary>
    /// 缓存类型
    /// </summary>
    public enum CacheTypes
    {
        /// <summary>
        /// 使用内存缓存(不支持分布式)
        /// </summary>
        InMemory = 0,

        /// <summary>
        /// 使用Redis缓存(支持分布式)
        /// </summary>
        Redis = 1
    }
}
