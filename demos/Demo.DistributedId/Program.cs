using Colder.Cache;
using Colder.DistributedId;
using Colder.DistributedLock.Abstractions;
using Colder.DistributedLock.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.DistributedId
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Task.CompletedTask;

            IServiceCollection services = new ServiceCollection();
            services.AddDistributedLock(x =>
            {
                x.LockType = LockTypes.InMemory;
                x.RedisEndPoints = new string[] { "localhost:6379" };
            });
            services.AddCache(new CacheOptions
            {
                CacheType = CacheTypes.Redis,
                RedisConnectionString = "localhost:6379"
            });
            services.AddDistributedId(new DistributedIdOptions
            {
                Distributed = true
            });

            var serviceProvider = services.BuildServiceProvider();
            var distributedId = serviceProvider.GetRequiredService<IDistributedId>();
            var ids = Enumerable.Range(0, 10000).Select(x => distributedId.NewLongId()).ToList();

            Console.ReadLine();
        }
    }
}
