using Colder.Common;
using Colder.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
                var cacheOption = buidlerContext.Configuration.GetSection("cache").Get<CacheOptions>();

                services.AddCache(cacheOption);
            });

            return hostBuilder;
        }

        /// <summary>
        /// 注入缓存
        /// </summary>
        /// <param name="services"></param>
        /// <param name="cacheOption"></param>
        /// <returns></returns>
        public static IServiceCollection AddCache(this IServiceCollection services, CacheOptions cacheOption)
        {
            switch (cacheOption.CacheType)
            {
                case CacheTypes.InMemory: services.AddDistributedMemoryCache(); break;
                case CacheTypes.Redis:
                    {
                        services.AddStackExchangeRedisCache(options =>
                        {
                            options.Configuration = cacheOption.RedisConnectionString;
                        });
                    }; break;
                default: throw new Exception("缓存类型无效");
            }

            return services;
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
            options ??= new DistributedCacheEntryOptions();

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
            options ??= new DistributedCacheEntryOptions();

            await distributedCache.SetStringAsync(cacheKey, SerializeObject(value), options);
        }

        private static string SerializeObject(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            var type = obj.GetType();

            if (obj.GetType().IsSimple())
            {
                if (type == typeof(DateTime) || type == typeof(DateTime?))
                {
                    return ((DateTime)obj).ToString("o");
                }
                else if (type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?))
                {
                    return ((DateTimeOffset)obj).ToString("o");
                }

                return obj?.ToString();
            }
            else
            {
                return obj.ToJson(false);
            }
        }

        private static T DeserializeObject<T>(string json)
        {
            if (typeof(T).IsSimple())
            {
                if (typeof(T) == typeof(DateTime) || typeof(T) == typeof(DateTime?))
                {
                    return (T)(object)DateTime.Parse(json);
                }
                else if (typeof(T) == typeof(DateTimeOffset) || typeof(T) == typeof(DateTimeOffset?))
                {
                    return (T)(object)DateTimeOffset.Parse(json);
                }
                else if (typeof(T) == typeof(Guid) || typeof(T) == typeof(Guid?))
                {
                    return (T)(object)Guid.Parse(json);
                }

                return (T)Convert.ChangeType(json, typeof(T));
            }
            else
            {
                return json.ToObject<T>(false);
            }
        }
    }
}
