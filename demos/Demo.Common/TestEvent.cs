using Colder.MessageBus.Abstractions;

namespace Demo.Common
{
    public class TestEvent : IEvent
    {
        public string Text { get; set; }
    }
}
