using System;
using Lykke.Common.Chaos;
using Lykke.Sdk.Settings;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.HftInternalService.Core
{
    public class AppSettings : BaseAppSettings
    {
        public HftInternalServiceSettings HftInternalService { get; set; }
        public HighFrequencyTradingSettings HighFrequencyTradingService { get; set; }
        public ClientAccountServiceClient ClientAccountServiceClient { get; set; }
        public HftJwtAuthSettings HftJwtAuth { get; set; }
    }

    public class HftInternalServiceSettings
    {
        public DbSettings Db { get; set; }
        public string QueuePostfix { get; set; }
        public TimeSpan RetryDelay { get; set; }
        public string SagasRabbitMqConnStr { get; set; }
        [Optional]
        public ChaosSettings ChaosKitty { get; set; }

        public RabbitMqSettings Rabbit { get; set; }
    }

    public class HighFrequencyTradingSettings
    {
        public MongoSettings MongoSettings { get; set; }
    }

    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }
    }

    public class MongoSettings
    {
        public string ConnectionString { get; set; }
    }

    public class ClientAccountServiceClient
    {
        [HttpCheck("api/isAlive")]
        public string ServiceUrl { get; set; }
    }

    public class HftJwtAuthSettings
    {
        public string JwtSecret { get; set; }
        public string JwtAud { get; set; }
    }

    public class RabbitMqSettings
    {
        public string ConnectionString { get; set; }
        public string Namespace { get; set; }
        public string ExchangeName { get; set; }
    }
}
