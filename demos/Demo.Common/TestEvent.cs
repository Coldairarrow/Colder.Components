using Colder.MessageBus.Abstractions;

namespace Demo.Common
{
    public class TestEvent : IEvent, ICommand
    {
        public string Text { get; set; }
    }
}
