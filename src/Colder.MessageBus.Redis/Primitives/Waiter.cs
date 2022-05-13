using System.Threading;

namespace Colder.MessageBus.Redis
{
    internal class Waiter
    {
        public Semaphore Sp { get; set; }
        public string ResponseJson { get; set; }
    }
}
