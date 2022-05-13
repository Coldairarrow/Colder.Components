using Colder.MessageBus.Abstractions;
using Demo.Common;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Demo.MessageBus.Consumer
{
    class Handler : IMessageHandler<RequestMessage>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IMessageBus _messageBus;
        public Handler(ILogger<Handler> logger, IMessageBus messageBus)
        {
            _logger = logger;
            _messageBus = messageBus;
        }
        public async Task Handle(MessageContext<RequestMessage> context)
        {
            _logger.LogInformation("收到 {EventType} 事件,MessageBody:{MessageBody}",
                typeof(RequestMessage).Name, JsonConvert.SerializeObject(context.Message));

            await Task.CompletedTask;
        }
    }
}
