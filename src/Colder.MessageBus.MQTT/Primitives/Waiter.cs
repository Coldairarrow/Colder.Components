using System.Threading;

namespace Colder.MessageBus.MQTT
{
    internal class Waiter
    {
        public Semaphore Sp { get; set; }
        public string ResponseJson { get; set; }
    }
}
