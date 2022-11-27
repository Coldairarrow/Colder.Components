using Colder.Common;
using Colder.EFCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Colder.EFCore;

/// <summary>
/// EFCore拓展
/// </summary>
public static class EFCoreExtentions
{
    /// <summary>
    /// 排序
    /// </summary>
    /// <typeparam name="TSource">泛型</typeparam>
    /// <param name="source">数据源</param>
    /// <param name="orderByFields">排序字段</param>
    /// <returns></returns>
    public static IQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, params OrderByField[] orderByFields)
    {
        var q = source;

        if (orderByFields?.Length > 0)
        {
            for (var i = 0; i < orderByFields.Length; i++)
            {
                var aOrder = orderByFields[i];
                var fieldName = aOrder.Field;

                if (!source.ElementType.GetMembers().Any(x => x.Name == fieldName))
                {
                    throw new Exception($"排序字段 {aOrder.Field} 无效");
                }

                var orderType = aOrder.Type == OrderByTypes.Desc ? "desc" : "asc";
                var orderString = $@"{fieldName} {orderType}";

                if (i == 0)
                {
                    q = q.OrderBy(orderString);
                }
                else
                {
                    q = ((IOrderedQueryable<TSource>)q).ThenBy(orderString);
                }
            }
        }

        return q;
    }

    /// <summary>
    /// 分页
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source">数据</param>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">每页数量</param>
    /// <returns></returns>
    public static IQueryable<TSource> Page<TSource>(this IQueryable<TSource> source, int pageIndex = 1, int pageSize = 30)
    {
        return source.Skip((pageIndex - 1) * pageSize).Take(pageSize);
    }

    /// <summary>
    /// 获取分页列表
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source">数据</param>
    /// <param name="pageIndex">页码,从1开始计数</param>
    /// <param name="pageSize">每页数量</param>
    /// <returns></returns>
    public static async Task<PageList<TSource>> ToPageList<TSource>(this IQueryable<TSource> source, int pageIndex, int pageSize)
    {
        var total = await source.CountAsync();
        source = source.Page(pageIndex, pageSize);
        return new PageList<TSource>
        {
            Items = await source.ToArrayAsync(),
            Total = total
        };
    }

    /// <summary>
    /// 获取分页列表
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source">数据</param>
    /// <param name="pageInput"></param>
    /// <returns></returns>
    public static async Task<PageList<TSource>> ToPageList<TSource>(this IQueryable<TSource> source, PageInput pageInput)
    {
        return await source.ToPageList(pageInput.PageIndex, pageInput.PageRows);
    }
}