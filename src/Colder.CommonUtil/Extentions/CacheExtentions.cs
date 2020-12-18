using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Colder.CommonUtil
{
    public static class CacheExtentions
    {
        public static async Task<T> GetOrSet<T>(this IDistributedCache distributedCache, string cacheKey, Func<Task<T>> getFromDb)
        {
            var cache = await distributedCache.GetObjectAsync<T>(cacheKey);
            if (cache.IsNullOrEmpty() || cache.Equals(default(T)))
            {
                cache = await getFromDb();
                await distributedCache.SetObjectAsync(cacheKey, cache);
            }

            return cache;
        }
    }
}
