using System;
using System.Threading.Tasks;
using Common.Log;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.HftInternalService.Contracts.Events;
using Lykke.Service.HftInternalService.Core;
using Lykke.Service.HftInternalService.Core.Services;

namespace Lykke.Service.HftInternalService.Services
{
    public class ApiKeyPublisher : IApiKeyPublisher
    {
        private readonly ILog _log;
        private readonly RabbitMqSettings _rabbitSettings;

        private RabbitMqPublisher<ApiKeyUpdatedMessage> _publisher;

        public ApiKeyPublisher(ILog log, RabbitMqSettings rabbitSettings)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _rabbitSettings = rabbitSettings ?? throw new ArgumentNullException(nameof(rabbitSettings));
        }

        public void Start()
        {
            var settings = new RabbitMqSubscriptionSettings
            {
                ConnectionString = _rabbitSettings.ConnectionString,
                ExchangeName = _rabbitSettings.ExchangeName
            };

            _publisher = new RabbitMqPublisher<ApiKeyUpdatedMessage>(settings)
                .SetSerializer(new JsonMessageSerializer<ApiKeyUpdatedMessage>())
                .SetPublishStrategy(new DefaultFanoutPublishStrategy(settings))
                .PublishSynchronously()
                .SetLogger(_log)
                .Start();
        }

        public void Dispose()
        {
            _publisher?.Dispose();
        }

        public void Stop()
        {
            _publisher?.Stop();
        }

        public async Task PublishAsync(ApiKeyUpdatedMessage message)
        {
            await _publisher.ProduceAsync(message);
        }
    }
}
