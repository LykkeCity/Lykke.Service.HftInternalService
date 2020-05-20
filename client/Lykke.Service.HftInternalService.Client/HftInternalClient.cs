using Lykke.HttpClientGenerator;
using Lykke.Service.HftInternalService.Client.Api;

namespace Lykke.Service.HftInternalService.Client
{
    /// <summary>
    /// Hft internal API aggregating interface.
    /// </summary>
    public class HftInternalClient
    {
        public IKeysApi Keys { get; private set; }

        public HftInternalClient(IHttpClientGenerator httpClientGenerator)
        {
            Keys = httpClientGenerator.Generate<IKeysApi>();
        }
    }
}
