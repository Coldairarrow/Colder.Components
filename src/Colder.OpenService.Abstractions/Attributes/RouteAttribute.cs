using System;

namespace Colder.OpenService.Abstractions
{
    /// <summary>
    /// 仅支持简单路由,禁用api/[controller]这种高级路由
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false)]
    public class RouteAttribute : Attribute
    {
        /// <summary>
        /// 沟站函数
        /// </summary>
        /// <param name="template">路由</param>
        public RouteAttribute(string template)
        {
            Template = template;
        }

        /// <summary>
        /// 路由
        /// </summary>
        public string Template { get; }
    }
}
