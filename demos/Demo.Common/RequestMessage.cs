using Colder.MessageBus.Abstractions;

namespace Demo.Common
{
    public class RequestMessage : IMessage
    {
        public string Text { get; set; }
    }
}
