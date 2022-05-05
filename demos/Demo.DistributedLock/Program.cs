using Colder.DistributedLock.Abstractions;
using Colder.DistributedLock.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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
                x.LockType = LockTypes.InMemory;
            });

            var serviceProvider = services.BuildServiceProvider();

            var theLock = serviceProvider.GetService<IDistributedLock>();
            string key = Guid.NewGuid().ToString();

            List<Task> tasks = new List<Task>();

            int num = 0;

            for (int i = 0; i < 4; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    for (int j = 0; j < 10000; j++)
                    {
                        using var _ = await theLock.Lock(key);
                        num++;
                        Console.WriteLine(num);
                    }
                }));
            }

            await Task.WhenAll(tasks);

            Console.ReadLine();
        }
    }
}
