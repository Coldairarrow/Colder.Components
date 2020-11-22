using Castle.DynamicProxy;
using Colder.MessageBus.Abstractions;
using Dynamitey;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Colder.MessageBus.Hosting
{
    /// <summary>
    /// 消息总线拓展
    /// </summary>
    public static class DependencyInjectionExtentions
    {
        #region 私有成员

        private static readonly ProxyGenerator Generator = new ProxyGenerator();
        private class ActLikeInterceptor : IInterceptor
        {
            public ActLikeInterceptor(object obj)
            {
                _obj = obj;
            }
            private object _obj;
            public void Intercept(IInvocation invocation)
            {
                var name = new InvokeMemberName(invocation.Method.Name, invocation.Method.GetGenericArguments());

                if (invocation.Method.ReturnType != typeof(void))
                {
                    invocation.ReturnValue = Dynamic.InvokeMember(_obj, name, invocation.Arguments);
                }
                else
                {
                    Dynamic.InvokeMemberAction(_obj, name, invocation.Arguments);
                }
            }
        }

        #endregion

        #region 外部接口

        /// <summary>
        /// 配置消息总线
        /// </summary>
        /// <param name="hostBuilder">宿主构造</param>
        /// <returns></returns>
        public static IHostBuilder ConfigureMessageBusDefaults(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((host, services) =>
            {
                var configuration = host.Configuration;
                var options = configuration.GetSection("messagebus").Get<MessageBusOptions>();
                if (options == null)
                {
                    throw new Exception("消息总线初始化异常：缺少配置信息");
                }

                services.AddMessageBus<IMessageBus>(options);
            });

            return hostBuilder;
        }

        /// <summary>
        /// 注入消息总线
        /// </summary>
        /// <param name="services">服务容器</param>
        /// <param name="options">配置</param>
        /// <returns></returns>
        public static IServiceCollection AddMessageBus(this IServiceCollection services, MessageBusOptions options)
        {
            return AddMessageBus<IMessageBus>(services, options);
        }

        /// <summary>
        /// 注入自定义消息总线
        /// 注：可以通过此方法注入多个总线
        /// </summary>
        /// <typeparam name="TMessageBus">自定义总线类型</typeparam>
        /// <param name="services">服务容器</param>
        /// <param name="options">配置</param>
        /// <returns></returns>
        public static IServiceCollection AddMessageBus<TMessageBus>(this IServiceCollection services, MessageBusOptions options)
            where TMessageBus : class, IMessageBus
        {
            if(!typeof(TMessageBus).IsPublic)
            {
                throw new Exception($"{typeof(TMessageBus).FullName}必须公开");
            }

            services.AddHostedService<MessageBusBootstraper>();
            MessageBusBootstraper.Bootstrap += serviceProvider => serviceProvider.GetService<TMessageBus>();

            return services.AddSingleton(serviceProvider =>
            {
                var busInstance = MessageBusFactory.GetBusInstance(serviceProvider, options);

                if (typeof(TMessageBus) == typeof(IMessageBus))
                    return (TMessageBus)busInstance;
                else
                    return Generator.CreateInterfaceProxyWithoutTarget<TMessageBus>(new ActLikeInterceptor(busInstance));
            });
        }

        #endregion
    }
}
