using System;
using System.Collections.Concurrent;

namespace Colder.MessageBus.MQTT
{
    internal static class RequestWaiter
    {
        public static ConcurrentDictionary<Guid, Waiter> WaitingDic 
            = new ConcurrentDictionary<Guid, Waiter>();
    }
}
