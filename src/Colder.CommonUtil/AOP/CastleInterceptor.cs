using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Colder.CommonUtil
{
    internal class CastleInterceptor : AsyncInterceptorBase
    {
        private readonly IServiceProvider _serviceProvider;
        public CastleInterceptor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private List<BaseAOPAttribute> _aops;
        private int _depth;
        private readonly ConcurrentDictionary<int, IAOPContext> _contextDic = new ConcurrentDictionary<int, IAOPContext>();
        private readonly ConcurrentDictionary<int, Stopwatch> _timeDic = new ConcurrentDictionary<int, Stopwatch>();

        private async Task Befor()
        {
            _timeDic[_depth] = Stopwatch.StartNew();

            foreach (var aAop in _aops)
            {
                await aAop.Befor(_contextDic[_depth]);
            }
        }
        private async Task After()
        {
            var watch = _timeDic[_depth];
            watch.Stop();

            var context = _contextDic[_depth];
            //只记录超过1秒的接口
            if (watch.ElapsedMilliseconds >= 1000)
            {
                var logger = _serviceProvider.GetService<ILoggerFactory>()?.CreateLogger(GetType());

                logger?.LogInformation("执行方法 {Method} 耗时 {ElapsedMilliseconds}ms",
                    $"{context.Method.DeclaringType.FullName}.{context.Method.Name}", watch.ElapsedMilliseconds);
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
