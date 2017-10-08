namespace Lykke.Service.HftInternalService.Core
{
    public class AppSettings
    {
        public HftInternalServiceSettings HftInternalService { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
        public HighFrequencyTradingSettings HighFrequencyTradingService { get; set; }
    }

    public class HftInternalServiceSettings
    {
        public DbSettings Db { get; set; }
        public string ClientAccountServiceApiUrl { get; set; }
    }

    public class HighFrequencyTradingSettings
    {
        public CacheSettings CacheSettings { get; set; }
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

    public class CacheSettings
    {
        public string RedisConfiguration { get; set; }

        public string ApiKeyCacheInstance { get; set; }
        public string ApiKeyCacheKeyPattern { get; set; }
    }

    public static class CacheSettingsExt
    {
        public static string GetApiKey(this CacheSettings settings, string apiKey)
        {
            return string.Format(settings.ApiKeyCacheKeyPattern, apiKey);
        }
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
}
