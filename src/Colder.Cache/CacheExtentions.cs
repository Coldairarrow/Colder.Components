using Colder.CommonUtil;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Colder.Cache
{
    /// <summary>
    /// 缓存拓展
    /// </summary>
    public static class CacheExtentions
    {
        /// <summary>
        /// 使用缓存
        /// </summary>
        /// <param name="hostBuilder">建造者</param>
        /// <returns></returns>
        public static IHostBuilder ConfigureCacheDefaults(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((buidlerContext, services) =>
            {
                var cacheOption = buidlerContext.Configuration.GetSection("Cache").Get<CacheOptions>();
                switch (cacheOption.CacheType)
                {
                    case CacheTypes.InMemory: services.AddDistributedMemoryCache(); break;
                    case CacheTypes.Redis:
                        {
                            //将Redis分布式缓存服务添加到服务中
                            services.AddStackExchangeRedisCache(options =>
                            {
                                //用于连接Redis的配置  Configuration.GetConnectionString("RedisConnectionString")读取配置信息的串
                                options.Configuration = "localhost";// Configuration.GetConnectionString("RedisConnectionString");
                                                                    //Redis实例名RedisDistributedCache
                                options.InstanceName = "RedisDistributedCache";
                            });
                        }; break;
                    default: throw new Exception("缓存类型无效");
                }
            });

            return hostBuilder;
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
        public static async Task<T> GetOrSetObjectAsync<T>(this IDistributedCache distributedCache, string cacheKey, Func<Task<T>> getFromDb, DistributedCacheEntryOptions options = null)
        {
            options = options ?? new DistributedCacheEntryOptions();

            T resObj;

            var body = await distributedCache.GetStringAsync(cacheKey);
            if (string.IsNullOrEmpty(body))
            {
                resObj = await getFromDb();
                await distributedCache.SetStringAsync(cacheKey, SerializeObject(resObj), options);
            }
            else
            {
                resObj = DeserializeObject<T>(body);
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
        public static async Task<T> GetObjectAsync<T>(this IDistributedCache distributedCache, string cacheKey)
        {
            var body = await distributedCache.GetStringAsync(cacheKey);
            if (string.IsNullOrEmpty(body))
            {
                return default;
            }
            else
            {
                return DeserializeObject<T>(body);
            }
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="distributedCache"></param>
        /// <param name="cacheKey"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static async Task SetObjectAsync<T>(this IDistributedCache distributedCache, string cacheKey, T value, DistributedCacheEntryOptions options = null)
        {
            await distributedCache.SetStringAsync(cacheKey, SerializeObject(value), options);
        }

        private static string SerializeObject(object obj)
        {
            if (obj.GetType().IsSimple())
            {
                return obj?.ToString();
            }
            else
            {
                return JsonConvert.SerializeObject(obj);
            }
        }

        private static T DeserializeObject<T>(string json)
        {
            if (typeof(T).IsSimple())
            {
                return (T)Convert.ChangeType(json, typeof(T));
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
        }
    }
}
