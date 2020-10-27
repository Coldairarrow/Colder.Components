namespace Colder.MessageBus.MQTT.Utils
{
    internal static class TopicHelper
    {
        //Topic格式
        //ClientId={Endpoint}.{MachineName}
        //Colder.MessageBus.MQTT/{ClientId}/{Endpoint}/{MessageType}/{MessageId}
        private static readonly string _rootTopic = "Colder.MessageBus.MQTT";

        public static string BuildTopic(string clientId,)
    }
}
