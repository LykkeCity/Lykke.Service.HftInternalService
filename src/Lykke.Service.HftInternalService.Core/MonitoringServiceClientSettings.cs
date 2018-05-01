using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.HftInternalService.Core
{
    public class MonitoringServiceClientSettings
    {
        [HttpCheck("api/isalive")]
        public string MonitoringServiceUrl { get; set; }
    }
}
