using System.Net;

namespace Lykke.Service.HftInternalService.Core
{
    public class AppSettings
    {
        public HftInternalServiceSettings HftInternalService { get; set; }
        public MatchingEngineSettings MatchingEngineClient { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
        public HighFrequencyTradingSettings HighFrequencyTradingService { get; set; }
    }

    public class HftInternalServiceSettings
    {
        public DbSettings Db { get; set; }
        public string ClientAccountServiceApiUrl { get; set; }
    }
    public class MatchingEngineSettings
    {
        public IpEndpointSettings IpEndpoint { get; set; }
    }

    public class IpEndpointSettings
    {
        public string InternalHost { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }

        public IPEndPoint GetClientIpEndPoint(bool useInternal = false)
        {
            string host = useInternal ? InternalHost : Host;

            IPAddress ipAddress;
            if (IPAddress.TryParse(host, out ipAddress))
                return new IPEndPoint(ipAddress, Port);

            var addresses = Dns.GetHostAddressesAsync(host).Result;
            return new IPEndPoint(addresses[0], Port);
        }
    }


    public class HighFrequencyTradingSettings
    {
        public string ApiKey { get; set; }
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
