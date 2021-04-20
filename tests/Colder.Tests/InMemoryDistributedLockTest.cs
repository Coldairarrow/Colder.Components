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
    public class InMemoryDistributedLockTest
    {
        static InMemoryDistributedLockTest()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddDistributedLock(x =>
            {
                x.LockType = LockTypes.InMemory;
            });

            _serviceProvider = services.BuildServiceProvider();
        }
        private static readonly IServiceProvider _serviceProvider;

        [TestMethod]
        public async Task Test()
        {
            var theLock = _serviceProvider.GetService<IDistributedLock>();
            string key = Guid.NewGuid().ToString();

            List<Task<Task>> tasks = new List<Task<Task>>();

            int num = 0;
            for (int i = 0; i < 16; i++)
            {
                tasks.Add(Task.Factory.StartNew(async () =>
                {
                    for (int j = 0; j < 10000; j++)
                    {
                        using var _ = await theLock.Lock(key);
                        num++;
                    }
                }, TaskCreationOptions.LongRunning));
            }

            foreach(var aTask in tasks)
            {
                await (await aTask);
            }

            Assert.AreEqual(num, 160000);
        }
    }
}
