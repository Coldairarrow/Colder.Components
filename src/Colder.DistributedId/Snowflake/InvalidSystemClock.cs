using System;

namespace Colder.DistributedId
{
    internal class InvalidSystemClock : Exception
    {      
        public InvalidSystemClock(string message) : base(message) { }
    }
}