namespace Colder.Orleans.Hosting
{
    /// <summary>
    /// Orleans提供器类型
    /// </summary>
    public enum ProviderTypes
    {
        /// <summary>
        /// 本地内存集群
        /// </summary>
        InMemory = 0,

        /// <summary>
        /// 使用AdoNet数据库集群
        /// </summary>
        AdoNet = 1
    }
}
