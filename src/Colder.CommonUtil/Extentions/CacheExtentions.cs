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
        public static async Task<T> GetOrSetAsync<T>(this IDistributedCache distributedCache, string cacheKey, Func<Task<T>> getFromDb, DistributedCacheEntryOptions options = null)
        {
            options = options ?? new DistributedCacheEntryOptions();

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
        /// 获取缓存，若不存在则设置缓存
        /// </summary>
        /// <typeparam name="T">缓存对象类型</typeparam>
        /// <param name="distributedCache">分布式缓存对象</param>
        /// <param name="cacheKey">缓存键值</param>
        /// <param name="getFromDb">从数据持久层获取数据</param>
        /// <param name="options">缓存参数</param>
        /// <returns></returns>
        public static T GetOrSet<T>(this IDistributedCache distributedCache, string cacheKey, Func<T> getFromDb, DistributedCacheEntryOptions options = null)
        {
            options = options ?? new DistributedCacheEntryOptions();

            T resObj;

            var body = distributedCache.GetString(cacheKey);
            if (string.IsNullOrEmpty(body))
            {
                resObj = getFromDb();
                distributedCache.SetString(cacheKey, JsonConvert.SerializeObject(resObj), options);
            }
            else
            {
                resObj = JsonConvert.DeserializeObject<T>(body);
            }

            return resObj;
        }

        /// <summary>
        /// 获取缓存，若不存在则设置缓存
        /// </summary>
        /// <param name="distributedCache">分布式缓存对象</param>
        /// <param name="cacheKey">缓存键值</param>
        /// <param name="getFromDb">从数据持久层获取数据</param>
        /// <param name="options">缓存参数</param>
        /// <returns></returns>
        public static async Task<string> GetOrSetAsync(this IDistributedCache distributedCache, string cacheKey, Func<Task<string>> getFromDb, DistributedCacheEntryOptions options = null)
        {
            options = options ?? new DistributedCacheEntryOptions();

            string value;

            var body = await distributedCache.GetStringAsync(cacheKey);
            if (string.IsNullOrEmpty(body))
            {
                value = await getFromDb();
                await distributedCache.SetStringAsync(cacheKey, value, options);
            }
            else
            {
                value = body;
            }

            return value;
        }

        /// <summary>
        /// 获取缓存，若不存在则设置缓存
        /// </summary>
        /// <param name="distributedCache">分布式缓存对象</param>
        /// <param name="cacheKey">缓存键值</param>
        /// <param name="getFromDb">从数据持久层获取数据</param>
        /// <param name="options">缓存参数</param>
        /// <returns></returns>
        public static string GetOrSet(this IDistributedCache distributedCache, string cacheKey, Func<string> getFromDb, DistributedCacheEntryOptions options = null)
        {
            options = options ?? new DistributedCacheEntryOptions();

            string value;

            var body = distributedCache.GetString(cacheKey);
            if (string.IsNullOrEmpty(body))
            {
                value = getFromDb();
                distributedCache.SetString(cacheKey, value, options);
            }
            else
            {
                value = body;
            }

            return value;
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T">缓存对象类型</typeparam>
        /// <param name="distributedCache">分布式缓存对象</param>
        /// <param name="cacheKey">缓存键值</param>
        /// <returns></returns>
        public static async Task<T> GetAsync<T>(this IDistributedCache distributedCache, string cacheKey)
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

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T">缓存对象类型</typeparam>
        /// <param name="distributedCache">分布式缓存对象</param>
        /// <param name="cacheKey">缓存键值</param>
        /// <returns></returns>
        public static T Get<T>(this IDistributedCache distributedCache, string cacheKey)
        {
            var body = distributedCache.GetString(cacheKey);
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
