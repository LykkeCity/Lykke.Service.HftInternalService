using System;
using Lykke.Common.Chaos;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.HftInternalService.Core
{
    public class AppSettings
    {
        public HftInternalServiceSettings HftInternalService { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
        public HighFrequencyTradingSettings HighFrequencyTradingService { get; set; }
        public ClientAccountServiceClient ClientAccountServiceClient { get; set; }
    }

    public class HftInternalServiceSettings
    {
        public DbSettings Db { get; set; }
        public string QueuePostfix { get; set; }
        public TimeSpan RetryDelay { get; set; }
        public string SagasRabbitMqConnStr { get; set; }
        [Optional]
        public ChaosSettings ChaosKitty { get; set; }
    }

    public class HighFrequencyTradingSettings
    {
        public MongoSettings MongoSettings { get; set; }
    }

    public class DbSettings
    {
        public string LogsConnString { get; set; }
    }

    public class MongoSettings
    {
        public string ConnectionString { get; set; }
    }

    public class SlackNotificationsSettings
    {
        public AzureQueueSettings AzureQueue { get; set; }
    }

    public class AzureQueueSettings
    {
        public string ConnectionString { get; set; }

        public string QueueName { get; set; }
    }

    public class ClientAccountServiceClient
    {
        public string ServiceUrl { get; set; }
    }
}
