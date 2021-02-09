using Colder.MessageBus.Abstractions;
using MediatR;
using MQTTnet;
using MQTTnet.Client;
using System;

namespace Colder.MessageBus.MQTT
{
    internal class MqttMessageReceivedEvent : INotification
    {
        public MqttApplicationMessageReceivedEventArgs EventArgs { get; set; }
        public IServiceProvider ServiceProvider { get; set; }
        public MessageBusOptions MessageBusOptions { get; set; }
        public IMqttClient MqttClient { get; set; }
    }
}
