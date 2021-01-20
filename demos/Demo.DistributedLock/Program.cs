using Colder.DistributedLock.Abstractions;
using Colder.DistributedLock.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Demo.DistributedLock
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddDistributedLock(x =>
            {
                x.LockType = LockTypes.Redis;
                x.RedisEndPoints = new string[] { "localhost:6379" };
            });
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "localhost:6379";
            });

            var serviceProvider = services.BuildServiceProvider();
            var cache = serviceProvider.GetService<IDistributedCache>();

            var theLock = serviceProvider.GetService<IDistributedLock>();
            string key = Assembly.GetCallingAssembly().GetName().Name;

            var numCacheKey = $"{key}.Num";
            int num = 0;

            await cache.SetStringAsync(numCacheKey, num.ToString());

            List<Task> tasks = new List<Task>();

            for (int i = 0; i < 4; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    for (int j = 0; j < int.MaxValue; j++)
                    {
                        using var _ = await theLock.Lock(key);

                        num = int.Parse(await cache.GetStringAsync(numCacheKey));
                        num++;

                        await cache.SetStringAsync(numCacheKey, num.ToString());

                        Console.WriteLine($"{DateTime.Now}:{num}");
                    }
                }));
            }

            await Task.WhenAll(tasks);
        }
    }
}
