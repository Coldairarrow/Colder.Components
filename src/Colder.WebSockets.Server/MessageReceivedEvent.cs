namespace Colder.WebSockets.Server
{
    internal class MessageReceivedEvent
    {
        public string ConnectionId { get; set; }
        public string Body { get; set; }
    }
}
