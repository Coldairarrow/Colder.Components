using Colder.MessageBus.Abstractions;
using Demo.Common;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Demo.MessageBus.Consumer
{
    class Handler : IMessageHandler<TestSubEvent>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IMessageBus _messageBus;
        public Handler(ILogger<Handler> logger, IMessageBus messageBus)
        {
            _logger = logger;
            _messageBus = messageBus;
        }
        public async Task Handle(IMessageContext<TestSubEvent> context)
        {
            _logger.LogInformation("收到 {EventType} 事件,MessageBody:{MessageBody}",
                typeof(TestSubEvent).Name, JsonConvert.SerializeObject(context.Message));

            //往回发送
            //var uri = new Uri($"rabbitmq://localhost/{MessageBusEndpoints.Producer}");
            await _messageBus.Publish(new TestSubEvent { Text = "回复" }, MessageBusEndpoints.Producer);
        }
    }
}
