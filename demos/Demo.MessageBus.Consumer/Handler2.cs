using Demo.Common;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace Demo.MessageBus.Consumer
{
    class Handler2 : IConsumer<TestEvent>
    {
        public Task Consume(ConsumeContext<TestEvent> context)
        {
            Console.WriteLine(context.MessageId);

            return Task.CompletedTask;
        }
    }
}
