using Castle.DynamicProxy;
using System;
using System.Threading.Tasks;

namespace Colder.Common.Util;

/// <summary>
/// 代理帮助类
/// </summary>
public static class ProxyHelper
{
    private static readonly ProxyGenerator _generator = new ProxyGenerator();

    /// <summary>
    /// 创建接口代理
    /// </summary>
    /// <typeparam name="TInterface">接口类型</typeparam>
    /// <param name="interface">接口对象</param>
    /// <param name="filter">过滤器，参数依次为上下文、执行委托</param>
    /// <returns></returns>
    public static TInterface CreateProxy<TInterface>(TInterface @interface, Func<IInvocation, Func<Task>, Task> filter)
        where TInterface : class
    {
        return _generator.CreateInterfaceProxyWithTarget(@interface, new CastleInterceptor(filter));
    }

    internal class CastleInterceptor : AsyncInterceptorBase
    {
        private readonly Func<IInvocation, Func<Task>, Task> _filter;
        public CastleInterceptor(Func<IInvocation, Func<Task>, Task> filter)
        {
            _filter = filter;
        }

        protected override async Task InterceptAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task> proceed)
        {
            await _filter(invocation, () => proceed(invocation, proceedInfo));
        }

        protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        {
            TResult result = default;

            await _filter(invocation, async () =>
            {
                result = await proceed(invocation, proceedInfo);
                if (typeof(Task).IsAssignableFrom(invocation.Method.ReturnType))
                {
                    invocation.ReturnValue = Task.FromResult(result);
                }
                else
                {
                    invocation.ReturnValue = result;
                }
            });

            return result;
        }
    }
}
