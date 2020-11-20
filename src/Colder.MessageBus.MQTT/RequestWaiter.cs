using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Colder.MessageBus.MQTT
{
    internal static class RequestWaiter
    {
        public static ConcurrentDictionary<Guid, (Semaphore sp, string responseJson)> WaitingDic 
            = new ConcurrentDictionary<Guid, (Semaphore sp, string responseJson)>();
    }
}
