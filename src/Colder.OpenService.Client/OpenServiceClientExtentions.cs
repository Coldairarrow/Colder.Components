using Colder.OpenService.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

namespace Colder.OpenService.Client
{
    /// <summary>
    /// OpenService客户端拓展
    /// </summary>
    public static class OpenServiceClientExtentions
    {
        /// <summary>
        /// OpenService服务
        /// </summary>
        /// <param name="services">容器</param>
        /// <param name="assembly">OpenService服务所在程序集</param>
        /// <param name="openServiceOption">参数</param>
        /// <returns></returns>
        public static IServiceCollection AddOpenServiceClient(this IServiceCollection services, Assembly assembly, OpenServiceOptions openServiceOption)
        {
            services.AddTransient<OpenServiceClientFactory>();

            var interfaces = assembly.GetTypes().Where(x => typeof(IOpenService).IsAssignableFrom(x)).ToList();

            //服务接口注入
            interfaces.ForEach(aInterface =>
            {
                services.AddTransient(aInterface, serviceProvider =>
                {
                    return serviceProvider.GetService<OpenServiceClientFactory>().GetClient(aInterface, openServiceOption);
                });
            });

            return services;
        }
    }
}
