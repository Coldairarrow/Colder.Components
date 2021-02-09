using Colder.MessageBus.Abstractions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Receiving;
using System;
using System.Threading.Tasks;

namespace Colder.MessageBus.MQTT
{
    internal class MessageBusHandler : IMqttApplicationMessageReceivedHandler
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly MessageBusOptions _messageBusOptions;
        private readonly IMqttClient _mqttClient;
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        public MessageBusHandler(
            IServiceProvider serviceProvider,
            MessageBusOptions messageBusOptions,
            IMqttClient mqttClient,
            IMediator mediator
            )
        {
            _serviceProvider = serviceProvider;
            _messageBusOptions = messageBusOptions;
            _mqttClient = mqttClient;
            _logger = _serviceProvider.GetService<ILogger<MessageBusHandler>>();
            _mediator = mediator;
        }
        public async Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
        {
            //这里为同步处理，使用MediatR转为异步，提高并发
            //https://github.com/chkr1011/MQTTnet/issues/812

            await _mediator.Publish(new MqttMessageReceivedEvent
            {
                EventArgs = e,
                MessageBusOptions = _messageBusOptions,
                MqttClient = _mqttClient,
                ServiceProvider = _serviceProvider
            });
        }
    }
}
