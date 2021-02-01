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
        private readonly IMemoryCache _lockDic = new MemoryCache(new MemoryCacheOptions());
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _keyDic
            = new ConcurrentDictionary<string, SemaphoreSlim>();
        public async Task<IDisposable> Lock(string key, TimeSpan? timeout)
        {
            SemaphoreSlim keyLock = null;
            try
            {
                timeout = timeout ?? TimeSpan.FromSeconds(10);

                UsingLock theLock;

                keyLock = _keyDic.GetOrAdd(key, new SemaphoreSlim(1, 1));
                await keyLock.WaitAsync(TimeSpan.FromSeconds(10));

                theLock = _lockDic.GetOrCreate(key, cacheEntry =>
                {
                    var newLock = new UsingLock();

                    cacheEntry.SlidingExpiration = TimeSpan.FromMinutes(10);
                    cacheEntry.PostEvictionCallbacks.Add(new PostEvictionCallbackRegistration
                    {
                        EvictionCallback = (_, _, _, _) =>
                        {
                            newLock.DisposeLock();
                            _keyDic.TryRemove(key, out _);
                            keyLock.Dispose();
                        }
                    });

                    return newLock;
                });

                keyLock.Release();

                await theLock.Wait(timeout.Value);

                return theLock;
            }
            catch
            {
                keyLock?.Release();

                throw new Exception($"获取锁失败 Key:{key}");
            }
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
