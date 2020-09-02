using System;
using System.Collections.Generic;

namespace Colder.MessageBus.Abstractions
{
    public interface IMessageContext
    {
        /// <summary>
        /// The messageId assigned to the message when it was initially Sent. This is different
        /// than the transport MessageId, which is only for the Transport.
        /// </summary>
        Guid? MessageId { get; }

        /// <summary>
        /// The address of the message producer that sent the message
        /// </summary>
        Uri SourceAddress { get; }

        /// <summary>
        /// The machine name (or role instance name) of the local machine
        /// </summary>
        string SourceMachineName { get; }

        /// <summary>
        /// The destination address of the message
        /// </summary>
        Uri DestinationAddress { get; }

        /// <summary>
        /// The response address to which responses to the request should be sent
        /// </summary>
        Uri ResponseAddress { get; }

        /// <summary>
        /// The fault address to which fault events should be sent if the message consumer faults
        /// </summary>
        Uri FaultAddress { get; }

        /// <summary>
        /// When the message was originally sent
        /// </summary>
        DateTime? SentTime { get; }

        /// <summary>
        /// Additional application-specific headers that are added to the message by the application
        /// or by features within MassTransit, such as when a message is moved to an error queue.
        /// </summary>
        Dictionary<string, object> Headers { get; }
    }
}
