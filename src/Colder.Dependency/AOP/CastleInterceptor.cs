using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Colder.Dependency
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
        private IAOPContext _context;
        private long _beginTime;

        private async Task Befor()
        {
            _beginTime = Stopwatch.GetTimestamp();

            foreach (var aAop in _aops)
            {
                await aAop.Befor(_context);
            }
        }
        private async Task After()
        {
            var elapsedMilliseconds = new TimeSpan((long)(TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency
                * (Stopwatch.GetTimestamp() - _beginTime))).TotalMilliseconds;

            //记录耗时方法
            if (elapsedMilliseconds >= _minElapsedMilliseconds)
            {
                var logger = _serviceProvider.GetService<ILoggerFactory>()?.CreateLogger(GetType());

                logger?.LogInformation("执行方法 {InvokeMethod} 耗时 {ElapsedMilliseconds:N}ms",
                    $"{_context.TargetType?.Name}.{_context.Method.Name}", elapsedMilliseconds);
            }

            foreach (var aAop in _aops)
            {
                await aAop.After(_context);
            }
        }
        private void Init(IInvocation invocation)
        {
            _context = new CastleAOPContext(invocation, _serviceProvider);

            _aops = invocation.MethodInvocationTarget.GetCustomAttributes(typeof(BaseAOPAttribute), true)
                .Concat(invocation.InvocationTarget.GetType().GetCustomAttributes(typeof(BaseAOPAttribute), true))
                .Select(x => (BaseAOPAttribute)x)
                .ToList();
        }

        protected override async Task InterceptAsync(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task> proceed)
        {
            Init(invocation);

            await Befor();
            await proceed(invocation, proceedInfo);
            await After();
        }

        protected override async Task<TResult> InterceptAsync<TResult>(IInvocation invocation, IInvocationProceedInfo proceedInfo, Func<IInvocation, IInvocationProceedInfo, Task<TResult>> proceed)
        {
            Init(invocation);

            TResult result;

            await Befor();
            result = await proceed(invocation, proceedInfo);
            await After();

            return result;
        }
    }
}
