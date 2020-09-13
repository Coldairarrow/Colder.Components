using Colder.MessageBus.Abstractions;
using Demo.Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Colder.CommonUtil.Tests
{
    internal class SendTest : BackgroundService
    {
        private readonly IMessageBus _messageBus;
        private readonly ILogger _logger;
        public SendTest(IMessageBus messageBus, ILogger<SendTest> logger)
        {
            _messageBus = messageBus;
            _logger = logger;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _ = new Timer(async state =>
            {
                await _messageBus.Publish(new TestSubEvent { Text = $"{DateTimeOffset.Now}Hi" });
                _logger.LogInformation($"已发送 {nameof(TestSubEvent)} 事件");
            }, null, 0, 1000);

            return Task.CompletedTask;
        }
    }
}
