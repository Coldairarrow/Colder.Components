using Colder.MessageBus.Abstractions;
using Demo.Common;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Demo.MessageBus.Gateway
{
    class Handler : IMessageHandler<RequestMessage>
    {
        private readonly ILogger<Handler> _logger;
        private readonly IMessageBus _messageBus;
        private readonly IConsumerMessageBus _consumerMessageBus;
        public Handler(ILogger<Handler> logger, IMessageBus messageBus, IConsumerMessageBus consumerMessageBus)
        {
            _logger = logger;
            _messageBus = messageBus;
            _consumerMessageBus = consumerMessageBus;
        }
        public async Task Handle(MessageContext<RequestMessage> context)
        {
            //_logger.LogInformation("收到 {EventType} 事件,MessageBody:{MessageBody}",
            //    typeof(RequestMessage).Name, JsonConvert.SerializeObject(context.Message));

            context.Response = await _consumerMessageBus.Request<RequestMessage, JObject>(context.Message, MessageBusEndpoints.Consumer);

            await Task.CompletedTask;
        }
    }
}
