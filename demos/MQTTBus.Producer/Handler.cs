using Colder.MessageBus.Abstractions;
using Demo.Common;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace MQTTBus.Producer
{
    public class Handler : IMessageHandler<TestEvent>
    {
        private readonly ILogger<Handler> _logger;
        public Handler(ILogger<Handler> logger)
        {
            _logger = logger;
        }
        public Task Handle(MessageContext<TestEvent> context)
        {
            _logger.LogInformation("收到 {EventType} 事件,MessageBody:{MessageBody}",
                typeof(TestEvent).Name, JsonConvert.SerializeObject(context.Message));

            return Task.CompletedTask;
        }
    }
}
