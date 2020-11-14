using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Colder.MessageBus.Hosting
{
    /// <summary>
    /// 消息总线初始化
    /// </summary>
    public class MessageBusBootstraper : BackgroundService
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public static Action<IServiceProvider> Bootstrap;

        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="serviceProvider"></param>
        public MessageBusBootstraper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override  Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Bootstrap?.Invoke(_serviceProvider);

            return Task.CompletedTask;
        }
    }
}
