using System;
using System.Reflection;

namespace Colder.Dependency
{
    /// <summary>
    /// AOP上下文
    /// </summary>
    public interface IAOPContext
    {
        /// <summary>
        /// 容器
        /// </summary>
        IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// 方法参数
        /// </summary>
        object[] Arguments { get; }

        /// <summary>
        /// 泛型参数类型
        /// </summary>
        Type[] GenericArguments { get; }

        /// <summary>
        /// 调用的方法
        /// </summary>
        MethodInfo Method { get; }

        /// <summary>
        /// 目标调用方法
        /// </summary>
        MethodInfo MethodInvocationTarget { get; }

        /// <summary>
        /// 代理对象
        /// </summary>
        object Proxy { get; }

        /// <summary>
        /// 返回值
        /// </summary>
        object ReturnValue { get; set; }

        /// <summary>
        /// 目标类型
        /// </summary>
        Type TargetType { get; }

        /// <summary>
        /// 目标对象
        /// </summary>
        object InvocationTarget { get; }
    }
}
