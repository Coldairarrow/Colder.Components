using Colder.Cache;
using Colder.DistributedId;
using Colder.DistributedLock.Abstractions;
using Colder.DistributedLock.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo.DistributedId
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddDistributedLock(x =>
            {
                x.LockType = LockTypes.InMemory;
            });
            services.AddCache(new CacheOptions
            {
                CacheType = CacheTypes.InMemory
            });
            services.AddDistributedId(new DistributedIdOptions { });

            var serviceProvider = services.BuildServiceProvider();
            var distributedId = serviceProvider.GetRequiredService<IDistributedId>();
            var ids = Enumerable.Range(0, 10000).Select(x => distributedId.NewLongId()).ToList();

            Console.ReadLine();
        }
    }
}
