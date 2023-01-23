using AsyncKeyedLock;
using Colder.DistributedLock.Abstractions;
using System;
using System.Threading.Tasks;

namespace Colder.DistributedLock.InMemory
{
    internal class InMemoryDistributedLock : IDistributedLock
    {
        private static readonly AsyncKeyedLocker<string> _asyncKeyedLocker = new(o =>
        {
            o.PoolSize = 20;
            o.PoolInitialFill = 1;
        });

        public async Task<IDisposable> Lock(string key, TimeSpan? timeout)
        {
            var releaser = (AsyncKeyedLockTimeoutReleaser<string>)await _asyncKeyedLocker.LockAsync(key, timeout ?? TimeSpan.FromSeconds(10)).ConfigureAwait(false);

            if (!releaser.EnteredSemaphore)
            {
                releaser.Dispose();
                throw new Exception("获取锁超时");
            }

            return releaser;
        }
    }
}
