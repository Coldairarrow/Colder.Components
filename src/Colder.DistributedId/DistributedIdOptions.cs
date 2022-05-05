namespace Colder.DistributedId
{
    /// <summary>
    /// 分布式Id选项
    /// </summary>
    public class DistributedIdOptions
    {
        /// <summary>
        /// Guid序列类型
        /// </summary>
        public SequentialGuidType GuidType { get; set; } = SequentialGuidType.AtBegin;

        /// <summary>
        /// 指定机器Id,范围1-1023,若不指定则在范围内随机取
        /// </summary>
        public int WorkderId { get; set; }

        /// <summary>
        /// 是否为分布式(即多实例部署)
        /// 若开启则需要提前配置分布式缓存(Colder.Cache)与分布式锁(Colder.DistributedLock)
        /// 多实例部署并且使用LongId(即雪花Id)时建议开启此选项
        /// </summary>
        public bool Distributed { get; set; }
    }
}
