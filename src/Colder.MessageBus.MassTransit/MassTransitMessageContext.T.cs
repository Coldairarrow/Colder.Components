using Colder.MessageBus.Abstractions;
using System;
using System.Collections.Generic;

namespace Colder.MessageBus.MassTransit
{
    internal class MassTransitMessageContext<TMessage> : IMessageContext<TMessage>
    {
        public TMessage Message { get; set; }
        public Guid? MessageId { get; set; }
        public Uri SourceAddress { get; set; }
        public string SourceMachineName { get; set; }
        public Uri DestinationAddress { get; set; }
        public Uri ResponseAddress { get; set; }
        public Uri FaultAddress { get; set; }
        public DateTime? SentTime { get; set; }
        public Dictionary<string, object> Headers { get; set; }
    }
}
