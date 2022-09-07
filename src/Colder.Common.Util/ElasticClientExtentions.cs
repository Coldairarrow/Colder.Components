using Nest;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Colder.Common.Util;

/// <summary>
/// ElasticClient拓展
/// </summary>
public static class ElasticClientExtentions
{
    /// <summary>
    /// 查询所有数据,通过SearchAfter方式
    /// </summary>
    /// <typeparam name="TDocument">文档类型</typeparam>
    /// <param name="elasticClient">es客户端</param>
    /// <param name="searchDescriptor">查询条件</param>
    /// <param name="idField">Id字段</param>
    /// <returns></returns>
    public static async Task<TDocument[]> SearchAll<TDocument>(
        this IElasticClient elasticClient,
        SearchDescriptor<TDocument> searchDescriptor = null,
        string idField = "Id"
        )
        where TDocument : class
    {
        List<TDocument> result = new List<TDocument>();
        TDocument last = null;
        while (true)
        {
            searchDescriptor ??= new SearchDescriptor<TDocument>();
            searchDescriptor = searchDescriptor.Sort(y => y.Ascending($"{idField}.keyword")).Size(10000);
            if (last != null)
            {
                searchDescriptor = searchDescriptor.SearchAfter(last.GetType().GetProperty(idField).GetValue(last));
            }

            var response = await elasticClient.SearchAsync<TDocument>(searchDescriptor);
            if (!response.ApiCall.Success)
            {
                throw response.OriginalException;
            }

            result.AddRange(response.Documents);

            if (response.Documents.Count < 10000)
            {
                break;
            }
            last = response.Documents.LastOrDefault();
        }

        return result.ToArray();
    }
}
