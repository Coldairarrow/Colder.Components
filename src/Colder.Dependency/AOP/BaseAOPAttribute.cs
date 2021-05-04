using System;
using System.Threading.Tasks;

namespace Colder.Dependency
{
    /// <summary>
    /// AOP基类
    /// 注:不支持控制器,需要定义接口并实现接口,自定义AOP特性放到接口实现类上
    /// </summary>
    public abstract class BaseAOPAttribute : Attribute
    {
        /// <summary>
        /// 执行方法前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public virtual async Task Befor(IAOPContext context)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// 执行方法后
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public virtual async Task After(IAOPContext context)
        {
            await Task.CompletedTask;
        }
    }
}
