using System.Threading.Tasks;

namespace Colder.MessageBus.Abstractions
{
    public interface IMessageHandler<TMessage> where TMessage : IMessage
    {
        Task Handle(IMessageContext<TMessage> context);
    }
}
