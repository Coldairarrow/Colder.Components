using Colder.MessageBus.Abstractions;
using Demo.Common;
using System;
using System.Threading.Tasks;

namespace Demo.MessageBus.Consumer
{
    class Handler : IMessageHandler<TestEvent>
    {
        public Task Handle(IMessageContext<TestEvent> context)
        {
            Console.WriteLine(context.MessageId);

            return Task.CompletedTask;
        }
    }
}
