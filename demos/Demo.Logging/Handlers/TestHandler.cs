using Colder.MessageBus.Abstractions;
using Demo.Common;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Demo.Logging.Handlers
{
    public class TestHandler : IMessageHandler<TestEvent>
    {
        private readonly ILogger _logger;
        public TestHandler(ILogger<TestHandler> logger)
        {
            _logger = logger;
        }
        public Task Handle(IMessageContext<TestEvent> context)
        {
            _logger.LogInformation("收到 {EventType} 事件:{Message}", typeof(TestEvent), JsonConvert.SerializeObject(context.Message));

            throw new System.Exception("11");

            return Task.CompletedTask;
        }
    }
}
