using System;

namespace Colder.Common
{
    /// <summary>
    /// 分页结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageList<T>
    {
        /// <summary>
        /// 结果
        /// </summary>
        public T[] Items { get; set; } = Array.Empty<T>();

        /// <summary>
        /// 总记录数
        /// </summary>
        public int Total { get; set; }
    }
}
