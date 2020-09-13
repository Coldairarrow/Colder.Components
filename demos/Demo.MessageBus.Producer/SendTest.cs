using Colder.MessageBus.Abstractions;
using Demo.Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.MessageBus.Producer
{
    internal class SendTest : BackgroundService
    {
        private readonly IMessageBus _messageBus;
        private readonly ILogger _logger;
        private Timer _timer;
        public SendTest(IMessageBus messageBus, ILogger<SendTest> logger)
        {
            _messageBus = messageBus;
            _logger = logger;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(async state =>
            {
                try
                {
                    await _messageBus.Publish(new TestSubEvent { Text = $"{DateTimeOffset.Now}Hi" });
                    _logger.LogInformation($"已发送 {nameof(TestSubEvent)} 事件");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "发送异常");
                }
            }, null, 0, 1000);

            return Task.CompletedTask;
        }
    }
}
