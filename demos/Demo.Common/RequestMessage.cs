using Colder.MessageBus.Abstractions;

namespace Demo.Common
{
    public class RequestMessage : IMessage
    {
        public string Text { get; set; }
        public int Index { get; set; }
        public int Total { get; set; }
    }
}
