using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Colder.Logging.Serilog
{
    internal class Bootstrapper : BackgroundService
    {
        private readonly ILogger _logger;
        public Bootstrapper(ILogger<Bootstrapper> logger)
        {
            _logger = logger;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, arg) =>
            {
                _logger.LogError((Exception)arg.ExceptionObject, "程序崩溃");
                //延迟三秒关闭程序，确保崩溃日志已写入完成
                Thread.Sleep(3000);
            };
            return Task.CompletedTask;
        }
    }
}
