using System;
using System.Threading.Tasks;

namespace Colder.DistributedLock.Abstractions
{
    /// <summary>
    /// 分布式锁
    /// </summary>
    public interface IDistributedLock
    {
        /// <summary>
        /// 获取锁(类似于lock(obj))
        /// </summary>
        /// <param name="key">锁键值</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        Task<IDisposable> Lock(string key, TimeSpan? timeout = null);
    }
}
