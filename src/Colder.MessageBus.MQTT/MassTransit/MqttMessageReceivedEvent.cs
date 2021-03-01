namespace Colder.MessageBus.MQTT
{
    internal class MqttMessageReceivedEvent
    {
        public string Topic { get; set; }
        public byte[] Payload { get; set; }
    }
}
