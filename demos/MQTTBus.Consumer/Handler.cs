using Colder.MessageBus.Abstractions;
using Demo.Common;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace MQTTBus.Consumer
{
    class Handler : IMessageHandler<TestEvent>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IMessageBus _messageBus;
        public Handler(ILogger<Handler> logger, IMessageBus messageBus)
        {
            _logger = logger;
            _messageBus = messageBus;
        }
        public async Task Handle(MessageContext<TestEvent> context)
        {
            _logger.LogInformation("收到 {EventType} 事件,MessageBody:{MessageBody}",
                typeof(TestSubEvent).Name, JsonConvert.SerializeObject(context.Message));

            //往回发送
            //var uri = new Uri($"rabbitmq://localhost/{MessageBusEndpoints.Producer}");
            await _messageBus.Publish(new TestEvent { Text = "回复" }, MessageBusEndpoints.Producer);
        }
    }
}
