using Colder.MessageBus.Abstractions;

namespace Demo.Common
{
    public class ResponseMessage : IMessage
    {
        public string Text { get; set; }
    }
}
