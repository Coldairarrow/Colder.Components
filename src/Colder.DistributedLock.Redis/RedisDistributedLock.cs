using Colder.DistributedLock.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Colder.DistributedLock.InMemory
{
    internal class RedisDistributedLock : IDistributedLock
    {
        public RedisDistributedLock(IOptions<DistributedLockOptions> options, IServiceProvider serviceProvider)
        {
            var multiplexers = options.Value.RedisEndPoints
                .Select(x => new RedLockMultiplexer(ConnectionMultiplexer.Connect(x)))
                .ToList();

            _redLockFactory = RedLockFactory.Create(multiplexers);

            IHostApplicationLifetime hostApplicationLifetime = serviceProvider.GetService<IHostApplicationLifetime>();
            if (hostApplicationLifetime != null)
            {
                hostApplicationLifetime.ApplicationStopping.Register(() =>
                {
                    _redLockFactory.Dispose();
                });
            }
        }

        private readonly RedLockFactory _redLockFactory;
        public async Task<IDisposable> Lock(string key, TimeSpan? timeout)
        {
            timeout = timeout ?? TimeSpan.FromSeconds(10);

            var expiry = TimeSpan.FromSeconds(30);
            var retry = TimeSpan.FromSeconds(1);

            var theLock = await _redLockFactory.CreateLockAsync(key, expiry, timeout.Value, retry);

            if (!theLock.IsAcquired)
            {
                throw new Exception($"获取分布式锁失败 Key:{key}");
            }

            return theLock;
        }
    }
}
