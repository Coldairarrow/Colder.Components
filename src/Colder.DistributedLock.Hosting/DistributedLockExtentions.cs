using Colder.DistributedLock.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Colder.DistributedLock.Hosting
{
    /// <summary>
    /// 分布式锁拓展
    /// </summary>
    public static class DistributedLockExtentions
    {
        /// <summary>
        /// 使用默认配置分布式锁
        /// </summary>
        /// <param name="hostBuilder">构造器</param>
        /// <returns></returns>
        public static IHostBuilder ConfigureDistributedLockDefaults(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((host, services) =>
            {
                DistributedLockOptions  distributedLockOptions=host.Configuration.GetSection("distributedLock").Get<DistributedLockOptions>();
            });

            return hostBuilder;
        }
    }
}
