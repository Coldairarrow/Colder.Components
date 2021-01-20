using Colder.DistributedLock.Abstractions;
using Colder.DistributedLock.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Colder.Tests
{
    [TestClass]
    public class RedisDistributedLockTest
    {
        static RedisDistributedLockTest()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddDistributedLock(x =>
            {
                x.LockType = LockTypes.Redis;
                x.RedisEndPoints = new string[] { "localhost:6379" };
            });

            _serviceProvider = services.BuildServiceProvider();
        }
        private static readonly IServiceProvider _serviceProvider;

        [TestMethod]
        public async Task Test()
        {
            var theLock = _serviceProvider.GetService<IDistributedLock>();
            string key = Guid.NewGuid().ToString();

            List<Task> tasks = new List<Task>();
            int num = 0;
            for (int i = 0; i < 4; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        using var _ = await theLock.Lock(key);
                        num++;
                    }
                }));
            }

            await Task.WhenAll(tasks);

            Assert.AreEqual(num, 4000);
        }
    }
}
