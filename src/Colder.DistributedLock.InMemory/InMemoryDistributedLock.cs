using System;
using System.Threading;
using System.Threading.Tasks;
using Colder.DistributedLock.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace Colder.DistributedLock.InMemory
{
    internal class InMemoryDistributedLock : IDistributedLock
    {
        private readonly IMemoryCache _lockDic = new MemoryCache(new MemoryCacheOptions());
        public async Task<IDisposable> Lock(string key, TimeSpan? timeout)
        {
            timeout = timeout ?? TimeSpan.FromSeconds(10);

            SemaphoreSlimLock theLock;

            lock (key)
            {
                theLock = _lockDic.GetOrCreate(key, cacheEntry =>
                {
                    var newLock = new SemaphoreSlimLock();

                    cacheEntry.SlidingExpiration = TimeSpan.FromMinutes(10);
                    cacheEntry.PostEvictionCallbacks.Add(new PostEvictionCallbackRegistration
                    {
                        EvictionCallback = (_, _, _, _) =>
                        {
                            newLock.DisposeLock();
                        }
                    });

                    return newLock;
                });
            }

            await theLock.Wait(timeout.Value);

            return theLock;
        }

        private class SemaphoreSlimLock : IDisposable
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
