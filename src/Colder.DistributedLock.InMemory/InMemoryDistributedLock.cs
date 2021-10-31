using Colder.DistributedLock.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Colder.DistributedLock.InMemory
{
    internal class InMemoryDistributedLock : IDistributedLock
    {
        private readonly IMemoryCache _lockDic = new MemoryCache(new MemoryCacheOptions()
        {
            //及时过期
            ExpirationScanFrequency = TimeSpan.FromSeconds(1)
        });
        private readonly ConcurrentDictionary<string, object> _internelLock = new ConcurrentDictionary<string, object>();
        public async Task<IDisposable> Lock(string key, TimeSpan? timeout)
        {
            timeout = timeout ?? TimeSpan.FromSeconds(10);

            UsingLock theLock;
            lock (_internelLock.GetOrAdd(key, new object()))
            {
                theLock = _lockDic.GetOrCreate(key, cacheEntry =>
                {
                    var newLock = new UsingLock();

                    cacheEntry.SlidingExpiration = TimeSpan.FromSeconds(30);
                    cacheEntry.RegisterPostEvictionCallback((_, _, reason, _) =>
                    {
                        if (reason == EvictionReason.Expired
                            || reason == EvictionReason.TokenExpired
                            || reason == EvictionReason.Removed)
                        {
                            newLock.DisposeLock();
                            _internelLock.TryRemove(key, out _);
                        }
                    });

                    return newLock;
                });
            }

            await theLock.Wait(timeout.Value);

            return theLock;
        }

        private class UsingLock : IDisposable
        {
            private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
            public async Task Wait(TimeSpan timeout)
            {
                if (!await _semaphore.WaitAsync(timeout))
                {
                    throw new Exception("获取锁超时");
                }
            }
            public void Dispose()
            {
                _semaphore.Release();
            }
            public void DisposeLock()
            {
                _semaphore.Dispose();
            }
        }
    }
}
