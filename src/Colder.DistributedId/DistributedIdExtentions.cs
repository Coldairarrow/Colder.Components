using Microsoft.Extensions.DependencyInjection;

namespace Colder.DistributedId
{
    /// <summary>
    /// 拓展
    /// </summary>
    public static class DistributedIdExtentions
    {
        /// <summary>
        /// 注入分布式Id
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="sequentialGuidType">排序类型</param>
        /// <returns></returns>
        public static IServiceCollection AddDistributedId(this IServiceCollection services, SequentialGuidType sequentialGuidType)
        {
            return services.AddSingleton<IDistributedId>(new DistributedId(sequentialGuidType));
        }
    }
}
