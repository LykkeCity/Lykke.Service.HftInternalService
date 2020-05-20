using JetBrains.Annotations;
using Lykke.Service.HftInternalService.Client.Api;

namespace Lykke.Service.HftInternalService.Client
{
    /// <summary>
    /// Hft internal client interface.
    /// </summary>
    [PublicAPI]
    public interface IHftInternalClient
    {
        /// <summary>Api for keys</summary>
        IKeysApi Keys { get; }
    }
}
