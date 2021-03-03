using Colder.MessageBus.Abstractions;
using Demo.Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.MessageBus
{
    class PublishService : BackgroundService
    {
        private readonly IMessageBus _messageBus;
        private Timer _timer;
        private readonly ILogger _logger;
        public PublishService(ISendOnlyMessageBus messageBus, ILogger<PublishService> logger)
        {
            _messageBus = messageBus;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _ = Task.Run(async () =>
            {
                for (int i = 0; i < 70000; i++)
                {
                    await _messageBus.Publish(new RequestMessage
                    {
                        Text = "小明",
                        Index = i + 1,
                        Total = 70000
                    });
                }

                _logger.LogInformation("发送完成");
            });

            await Task.CompletedTask;

            //_timer = new Timer(async state =>
            //{
            //    await _messageBus.Publish(new RequestMessage
            //    {
            //        Text = "小明"
            //    });
            //}, null, 0, 1000);
        }
    }
}
