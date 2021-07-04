using System;

namespace Colder.Common
{
    /// <summary>
    /// 分页查询基类
    /// </summary>
    public class PageInput
    {
        /// <summary>
        /// 当前页码，默认1
        /// </summary>
        public int PageIndex { get; set; } = 1;

        /// <summary>
        /// 每页行数，默认30
        /// </summary>
        public int PageRows { get; set; } = 30;

        /// <summary>
        /// 排序
        /// </summary>
        public OrderByField[] OrderBy { get; set; } = Array.Empty<OrderByField>();
    }

    /// <summary>
    /// 参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageInput<T> : PageInput where T : new()
    {
        /// <summary>
        /// 自定义参数
        /// </summary>
        public T Search { get; set; } = new T();
    }

    /// <summary>
    /// 排序字段
    /// </summary>
    public class OrderByField
    {
        /// <summary>
        /// 排序字段名
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// 排序类型
        /// </summary>
        public OrderByTypes Type { get; set; }
    }

    /// <summary>
    /// 排序类型
    /// </summary>
    public enum OrderByTypes
    {
        /// <summary>
        /// 顺序
        /// </summary>
        Asc = 0,

        /// <summary>
        /// 倒序
        /// </summary>
        Desc = 1
    }
}
