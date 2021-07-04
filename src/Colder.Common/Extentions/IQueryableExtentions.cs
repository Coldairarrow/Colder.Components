using System;
using System.Linq;
using System.Linq.Expressions;

namespace Colder.Common
{
    /// <summary>
    /// IQueryable"T"的拓展操作
    /// </summary>
    public static class IQueryableExtentions
    {
        /// <summary>
        /// 符合条件则Where
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="q">数据源</param>
        /// <param name="need">是否符合条件</param>
        /// <param name="where">筛选</param>
        /// <returns></returns>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> q, bool need, Expression<Func<T, bool>> where)
        {
            if (need)
            {
                return q.Where(where);
            }
            else
            {
                return q;
            }
        }
    }
}
