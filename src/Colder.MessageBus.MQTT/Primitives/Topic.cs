using System;
using System.Text.RegularExpressions;

namespace Colder.MessageBus.MQTT.Primitives
{
    internal class Topic
    {
        //Topic格式
        //MQTTMessageBus/{SourceClientId}/{TargetClientId}/{SourceEndpoint}/{TargetEndpoint}/{MessageBodyType}/{MessageType}/{MessageId}
        public static readonly string RootTopic = "MQTTMessageBus";

        public static Topic Parse(string topic)
        {
            string pattern = $"^{RootTopic}/(.*?)/(.*?)/(.*?)/(.*?)/(.*?)/(.*?)/(.*?)$";
            var match = Regex.Match(topic, pattern);

            Enum.TryParse<MessageTypes>(match.Groups[6].ToString(), out MessageTypes messageType);
            return new Topic
            {
                SourceClientId = match.Groups[1].ToString(),
                TargetClientId = match.Groups[2].ToString(),
                SourceEndpoint = match.Groups[3].ToString(),
                TargetEndpoint = match.Groups[4].ToString(),
                MessageBodyType = match.Groups[5].ToString(),
                MessageType = messageType,
                MessageId = Guid.Parse(match.Groups[7].ToString()),
            };
        }
        public string SourceClientId { get; set; }
        public string TargetClientId { get; set; }
        public string SourceEndpoint { get; set; }
        public string TargetEndpoint { get; set; }
        public string MessageBodyType { get; set; }
        public MessageTypes MessageType { get; set; }
        public Guid MessageId { get; set; }

        public override string ToString()
        {
            return $"{RootTopic}/{SourceClientId}/{TargetClientId}/{SourceEndpoint}/{TargetEndpoint}/{MessageBodyType}/{MessageType}/{MessageId}";
        }
    }
}
