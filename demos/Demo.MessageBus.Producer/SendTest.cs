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
                    var res = await _messageBus.Request<RequestMessage, ResponseMessage>(
                        new RequestMessage { Text = $"{DateTimeOffset.Now}Hi" }, MessageBusEndpoints.Consumer);
                    //_logger.LogInformation($"已发送 {nameof(RequestMessage)} 事件");
                    _logger.LogInformation("收到回复：{Response}", res.Text);
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
