using Colder.MessageBus.Abstractions;

namespace Demo.Common
{
    public class TestEvent : IMessage
    {
        public string Text { get; set; }
    }
}
