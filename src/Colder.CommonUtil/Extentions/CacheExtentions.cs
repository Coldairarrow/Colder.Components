using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Colder.CommonUtil
{
    /// <summary>
    /// 缓存拓展
    /// </summary>
    public static class CacheExtentions
    {
        /// <summary>
        /// 获取缓存，若不存在则设置缓存
        /// </summary>
        /// <typeparam name="T">缓存对象类型</typeparam>
        /// <param name="distributedCache">分布式缓存对象</param>
        /// <param name="cacheKey">缓存键值</param>
        /// <param name="getFromDb">从数据持久层获取数据</param>
        /// <param name="options">缓存参数</param>
        /// <returns></returns>
        public static async Task<T> GetOrSet<T>(this IDistributedCache distributedCache, string cacheKey, Func<Task<T>> getFromDb, DistributedCacheEntryOptions options = null)
        {
            T resObj;

            var body = await distributedCache.GetStringAsync(cacheKey);
            if (string.IsNullOrEmpty(body))
            {
                resObj = await getFromDb();
                await distributedCache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(resObj), options);
            }
            else
            {
                resObj = JsonConvert.DeserializeObject<T>(body);
            }

            return resObj;
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T">缓存对象类型</typeparam>
        /// <param name="distributedCache">分布式缓存对象</param>
        /// <param name="cacheKey">缓存键值</param>
        /// <returns></returns>
        public static async Task<T> Get<T>(this IDistributedCache distributedCache, string cacheKey)
        {
            var body = await distributedCache.GetStringAsync(cacheKey);
            if (string.IsNullOrEmpty(body))
            {
                return default;
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(body);
            }
        }
    }
}
