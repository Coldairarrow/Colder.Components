using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace Colder.DistributedId
{
    /// <summary>
    /// 拓展
    /// </summary>
    public static class DistributedIdExtentions
    {
        /// <summary>
        /// 使用默认配置分布式锁
        /// </summary>
        /// <param name="hostBuilder">构造器</param>
        /// <returns></returns>
        public static IHostBuilder ConfigureDistributedIdDefaults(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((host, services) =>
            {
                var option = host.Configuration.GetChildren()
                    .Where(x => x.Key.ToLower() == "distributedid")
                    .FirstOrDefault()
                    ?.Get<DistributedIdOptions>() ?? new DistributedIdOptions();

                services.AddDistributedId(option);
            });

            return hostBuilder;
        }

        /// <summary>
        /// 注入分布式Id
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="distributedIdOption"></param>
        /// <returns></returns>
        public static IServiceCollection AddDistributedId(this IServiceCollection services, DistributedIdOptions distributedIdOption)
        {
            services.AddOptions<DistributedIdOptions>().Configure(x =>
            {
                x.GetType().GetProperties().ToList().ForEach(aProperty =>
                {
                    aProperty.SetValue(x, aProperty.GetValue(distributedIdOption));
                });
            });

            return services.AddSingleton<IDistributedId, DistributedId>();
        }
    }
}
