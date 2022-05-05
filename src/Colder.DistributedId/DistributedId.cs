using Colder.DistributedLock.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Colder.DistributedId
{
    internal class DistributedId : IDistributedId
    {
        private IdWorker _idWorker;
        private readonly object _lock = new object();
        private readonly DistributedIdOptions _distributedIdOption;
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;
        public DistributedId(IOptions<DistributedIdOptions> distributedIdOption, IServiceProvider serviceProvider)
        {
            _distributedIdOption = distributedIdOption.Value;
            _serviceProvider = serviceProvider;
        }

        public Guid NewGuid()
        {
            return GuidHelper.NewGuid(_distributedIdOption.GuidType);
        }

        public Guid NewGuid(SequentialGuidType sequentialGuidType)
        {
            return GuidHelper.NewGuid(sequentialGuidType);
        }

        public long NewLongId()
        {
            if (_idWorker == null)
            {
                lock (_lock)
                {
                    if (_idWorker == null)
                    {
                        var workerId = AsyncHelper.RunSync(() => GetWorkerId());
                        _idWorker = new IdWorker(workerId);
                    }
                }
            }

            return _idWorker.NextId();
        }

        private async Task<int> GetWorkerId()
        {
            if (!_distributedIdOption.Distributed)
            {
                return _distributedIdOption.WorkderId == 0 ? new Random().Next(1, 1024) : _distributedIdOption.WorkderId;
            }

            //分布式需要计算WorkerId
            IDistributedCache distributedCache = _serviceProvider.GetService<IDistributedCache>();
            if (distributedCache == null)
            {
                throw new Exception("请注入IDistributedCache:https://github.com/Coldairarrow/Colder.Components#%E5%88%86%E5%B8%83%E5%BC%8F%E7%BC%93%E5%AD%98");
            }
            IDistributedLock distributedLock = _serviceProvider.GetService<IDistributedLock>();
            if (distributedLock == null)
            {
                throw new Exception("请注入IDistributedLock:https://github.com/Coldairarrow/Colder.Components#%E5%88%86%E5%B8%83%E5%BC%8F%E9%94%81");
            }

            for (int i = 1; i < 1024; i++)
            {
                var lockKey = $"{GetType().FullName}:WorkerIdLock:{i}";
                using var _ = distributedLock.Lock(lockKey);

                var key = $"{GetType().FullName}:WorkerId:{i}";
                var value = await distributedCache.GetStringAsync(key);
                if (string.IsNullOrEmpty(value))
                {
                    await distributedCache.SetStringAsync(key, DateTimeOffset.Now.ToString(), new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                    });

                    //定时刷新，心跳
                    _timer = new Timer(state =>
                    {
                        distributedCache.SetString(key, DateTimeOffset.Now.ToString(), new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                        });
                    }, null, 0, 1000);

                    return i;
                }
            }

            throw new Exception("WorkerId已用完");
        }
    }
}
