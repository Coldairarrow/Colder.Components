using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Colder.Dependency
{
    /// <summary>
    /// 自动注入服务扩展
    /// </summary>
    public static class DependencyExtentions
    {
        private static readonly ProxyGenerator _generator = new ProxyGenerator();

        /// <summary>
        /// 自动注入服务
        /// 服务必须继承ITransientDependency、IScopedDependency或ISingletonDependency
        /// </summary>
        /// <param name="services">容器</param>
        /// <param name="assemblies">注入程序集</param>
        /// <param name="minElapsedMilliseconds">调用方法耗时记录最小值，默认1000ms</param>
        /// <returns></returns>
        public static IServiceCollection AddServices(this IServiceCollection services, Assembly[] assemblies, int minElapsedMilliseconds = 1000)
        {
            Dictionary<Type, ServiceLifetime> lifeTimeMap = new Dictionary<Type, ServiceLifetime>
            {
                { typeof(ITransientDependency), ServiceLifetime.Transient},
                { typeof(IScopedDependency),ServiceLifetime.Scoped},
                { typeof(ISingletonDependency),ServiceLifetime.Singleton}
            };

            var allTypes = assemblies.SelectMany(x => x.GetTypes()).ToList();

            allTypes.ForEach(aImplementType =>
            {
                lifeTimeMap.ToList().ForEach(aMap =>
                {
                    var theDependency = aMap.Key;
                    if (theDependency.IsAssignableFrom(aImplementType) && theDependency != aImplementType && !aImplementType.IsAbstract && aImplementType.IsClass)
                    {
                        var interfaces = allTypes.Where(x => x.IsAssignableFrom(aImplementType) && x.IsInterface && x != theDependency).ToList();
                        //有接口则注入接口
                        if (interfaces.Count > 0)
                        {
                            interfaces.ForEach(aInterface =>
                            {
                                //注入AOP
                                services.Add(new ServiceDescriptor(aInterface, serviceProvider =>
                                {
                                    CastleInterceptor castleInterceptor = new CastleInterceptor(serviceProvider, minElapsedMilliseconds);

                                    return _generator.CreateInterfaceProxyWithTarget(
                                        aInterface,
                                        ActivatorUtilities.CreateInstance(serviceProvider, aImplementType),
                                        castleInterceptor);
                                }, aMap.Value));
                            });
                        }
                        //无接口直接注入自己
                        else
                        {
                            services.Add(new ServiceDescriptor(aImplementType, aImplementType, aMap.Value));
                        }
                    }
                });
            });

            return services;
        }
    }
}
