namespace Colder.DistributedLock.Abstractions
{
    /// <summary>
    /// 分布式锁类型
    /// </summary>
    public enum LockTypes
    {
        /// <summary>
        /// 内存实现(进程内有效)
        /// </summary>
        InMemory = 1,

        /// <summary>
        /// Redis实现(分布式)
        /// </summary>
        Redis = 2
    }
}
