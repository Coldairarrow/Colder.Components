using Colder.Cache;
using Colder.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Colder.Tests
{
    [TestClass]
    public class CacheTest
    {
        static CacheTest()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddCache(new CacheOptions
            {
                CacheType = CacheTypes.Redis,
                RedisConnectionString = "localhost:6379"
            });

            _serviceProvider = services.BuildServiceProvider();
        }
        private static readonly IServiceProvider _serviceProvider;

        [TestMethod]
        public async Task Test()
        {
            IDistributedCache distributedCache = _serviceProvider.GetService<IDistributedCache>();
            Dto dto = new Dto
            {
                Id = Guid.NewGuid(),
                Name = "小明",
                Time = DateTimeOffset.Now,
                Money = 100
            };

            string key = $"{GetType().FullName}:{nameof(Test)}";

            await distributedCache.RemoveAsync(key);

            await distributedCache.SetObjectAsync(key, dto);

            var newDto = await distributedCache.GetObjectAsync<Dto>(key);
            Assert.AreEqual(dto.ToJson(), newDto.ToJson());

            await distributedCache.SetObjectAsync(key, dto.Time);
            var cacheString = await distributedCache.GetStringAsync(key);
            Assert.AreEqual(dto.Time.ToString("o"), cacheString);
            var newTime = await distributedCache.GetObjectAsync<DateTimeOffset>(key);
            Assert.AreEqual(dto.Time.ToUnixTimeMilliseconds(), newTime.ToUnixTimeMilliseconds());

            await distributedCache.SetObjectAsync(key, dto.Id);
            cacheString = await distributedCache.GetStringAsync(key);
            Assert.AreEqual(dto.Id.ToString(), cacheString);
            var newId = await distributedCache.GetObjectAsync<Guid>(key);
            Assert.AreEqual(dto.Id, newId);

            await distributedCache.SetObjectAsync(key, dto.Money);
            cacheString = await distributedCache.GetStringAsync(key);
            Assert.AreEqual(dto.Money.ToString(), cacheString);
            var newMoney = await distributedCache.GetObjectAsync<decimal>(key);
            Assert.AreEqual(dto.Money, newMoney);
        }

        class Dto
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public DateTimeOffset Time { get; set; }
            public decimal Money { get; set; }
        }
    }
}
