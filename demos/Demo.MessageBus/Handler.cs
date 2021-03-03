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
            //_logger.LogInformation("收到 {EventType} 事件,MessageBody:{MessageBody}",
            //    typeof(RequestMessage).Name, JsonConvert.SerializeObject(context.Message));

            //await Task.Delay(1000);

            _logger.LogInformation("当前 {Index}/{Total}", context.Message.Index, context.Message.Total);
            //_logger.LogInformation("结束 {EventType} 事件,MessageBody:{MessageBody}",
            //    typeof(RequestMessage).Name, JsonConvert.SerializeObject(context.Message));

            await Task.CompletedTask;
        }
    }
}
