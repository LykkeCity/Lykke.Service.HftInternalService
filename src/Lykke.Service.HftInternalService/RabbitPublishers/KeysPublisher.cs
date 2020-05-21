using System;
using System.Threading.Tasks;
using Autofac;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.HftInternalService.Client.Messages;
using Lykke.Service.HftInternalService.Core;
using Lykke.Service.HftInternalService.Core.Services;

namespace Lykke.Service.HftInternalService.RabbitPublishers
{
    public class KeysPublisher : IKeysPublisher
    {
        private readonly RabbitMqSettings _rabbitMqSettings;
        private readonly ILogFactory _logFactory;
        private RabbitMqPublisher<KeyUpdatedEvent> _publisher;

        public KeysPublisher(
            RabbitMqSettings rabbitMqSettings,
            ILogFactory logFactory
            )
        {
            _rabbitMqSettings = rabbitMqSettings;
            _logFactory = logFactory;
        }

        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings
                .ForPublisher(_rabbitMqSettings.ConnectionString, _rabbitMqSettings.Namespace, _rabbitMqSettings.ExchangeName)
                .MakeDurable();

            _publisher = new RabbitMqPublisher<KeyUpdatedEvent>(_logFactory, settings)
                .SetSerializer(new JsonMessageSerializer<KeyUpdatedEvent>())
                .SetPublishStrategy(new DefaultFanoutPublishStrategy(settings))
                .PublishSynchronously()
                .Start();
        }

        public void Dispose()
        {
            _publisher?.Stop();
            _publisher?.Dispose();
        }

        public Task PublishAsync(KeyUpdatedEvent message)
        {
            return _publisher.ProduceAsync(message);
        }
    }
}
