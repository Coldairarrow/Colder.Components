using Colder.DistributedLock.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Reflection;

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
                services.Configure<DistributedLockOptions>(host.Configuration.GetSection("distributedLock"));
                services.AddDependency();
            });

            return hostBuilder;
        }

        /// <summary>
        /// 注入分布式锁
        /// </summary>
        /// <param name="services">注入容器</param>
        /// <param name="distributedLockOptionsBuilder">构造者</param>
        /// <returns></returns>
        public static IServiceCollection AddDistributedLock(this IServiceCollection services, Action<DistributedLockOptions> distributedLockOptionsBuilder = null)
        {
            services.AddOptions<DistributedLockOptions>();
            services.Configure(distributedLockOptionsBuilder);

            return services.AddDependency();
        }

        private static IServiceCollection AddDependency(this IServiceCollection services)
        {
            services.AddSingleton(typeof(IDistributedLock), serviceProvider =>
            {
                var options = serviceProvider.GetService<IOptions<DistributedLockOptions>>().Value;
                string assemblyName = $"Colder.DistributedLock.{options.LockType}";
                Type implementType;
                try
                {
                    implementType = Assembly.Load(assemblyName).GetTypes()
                        .Where(x => typeof(IDistributedLock).IsAssignableFrom(x))
                        .FirstOrDefault();
                }
                catch
                {
                    throw new Exception($"请安装包:{assemblyName}");
                }

                return ActivatorUtilities.CreateInstance(serviceProvider, implementType);
            });

            return services;
        }
    }
}
