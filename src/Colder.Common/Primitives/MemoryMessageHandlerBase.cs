using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Colder.Common;

/// <summary>
/// 内存消息队列处理
/// </summary>
/// <remarks>需要注入为单例</remarks>
public abstract class MemoryMessageHandlerBase
{
    private readonly BlockingCollection<object> _messages = new BlockingCollection<object>();
    private readonly Task[] _tasks;

    /// <summary>
    /// 
    /// </summary>
    protected readonly ILogger Logger;

    /// <summary>
    /// 
    /// </summary>
    protected readonly IServiceProvider ServiceProvider;

    /// <summary>
    /// 
    /// </summary>
    protected readonly IServiceScopeFactory ServiceScopeFactory;

    /// <summary>
    ///
    /// </summary>
    /// <param name="serviceProvider">serviceProvider</param>
    /// <param name="threads">线程数,默认为1</param>
    protected MemoryMessageHandlerBase(IServiceProvider serviceProvider, int threads = 1)
    {
        ServiceProvider = serviceProvider;
        ServiceScopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
        Logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(GetType());

        _tasks = Enumerable.Range(0, threads).Select(_ => Task.Factory.StartNew(async () =>
        {
            foreach (var message in _messages.GetConsumingEnumerable())
            {
                try
                {
                    var method = GetType().GetMethods((BindingFlags)63)
                        .Where(x => x.GetParameters().Length == 1 && x.GetParameters()[0].ParameterType == message.GetType())
                        .First();

                    if (method.Invoke(this, new object[] { message }) is Task task)
                    {
                        await task;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, ex.Message);
                }
            }
        }, TaskCreationOptions.LongRunning)).ToArray();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    public void AddMessage(object message)
    {
        _messages.Add(message);
    }
}

