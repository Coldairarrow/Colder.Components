using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Colder.CommonUtil
{
    internal class CastleInterceptor : AsyncInterceptorBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly int _minElapsedMilliseconds;
        public CastleInterceptor(IServiceProvider serviceProvider, int minElapsedMilliseconds)
        {
            _serviceProvider = serviceProvider;
            _minElapsedMilliseconds = minElapsedMilliseconds;
        }

        private List<BaseAOPAttribute> _aops;
        private int _depth;
        private readonly ConcurrentDictionary<int, IAOPContext> _contextDic = new ConcurrentDictionary<int, IAOPContext>();
        private readonly ConcurrentDictionary<int, long> _timeDic = new ConcurrentDictionary<int, long>();

        private async Task Befor()
        {
            _timeDic[_depth] = Stopwatch.GetTimestamp();

            foreach (var aAop in _aops)
            {
                await aAop.Befor(_contextDic[_depth]);
            }
        }
        private async Task After()
        {
            var elapsedMilliseconds = new TimeSpan((long)(TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency
                * (Stopwatch.GetTimestamp() - _timeDic[_depth]))).TotalMilliseconds;

            var context = _contextDic[_depth];
            //记录耗时方法
            if (elapsedMilliseconds >= _minElapsedMilliseconds)
            {
                var logger = _serviceProvider.GetService<ILoggerFactory>()?.CreateLogger(GetType());

                logger?.LogInformation("执行方法 {InvokeMethod} 耗时 {ElapsedMilliseconds:N}ms",
                    $"{context.TargetType?.Name}.{context.Method.Name}", elapsedMilliseconds);
            }

            foreach (var aAop in _aops)
            {
                await aAop.After(context);
            }

            _depth--;
        }
        private void Init(IInvocation invocation)
        {
            _depth++;

            _contextDic[_depth] = new CastleAOPContext(invocation, _serviceProvider);

            _aops = invocation.MethodInvocationTarget.GetCustomAttributes(typeof(BaseAOPAttribute), true)
                .Concat(invocation.InvocationTarget.GetType().GetCustomAttributes(typeof(BaseAOPAttribute), true))
                .Select(x => (BaseAOPAttribute)x)
                .ToList();
        }

        protected override async Task InterceptAsync(IInvocation invocation, Func<IInvocation, Task> proceed)
        {
            Init(invocation);

            await Befor();
            await proceed(invocation);
            await After();
        }

        protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, Func<IInvocation, Task<TResult>> proceed)
        {
            Init(invocation);

            TResult result;

            await Befor();
            result = await proceed(invocation);
            await After();

            return result;
        }
    }
}
