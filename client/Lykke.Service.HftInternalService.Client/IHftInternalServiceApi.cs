using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.HftInternalService.Client.Health;
using Refit;

namespace Lykke.Service.HftInternalService.Client
{
    /// <summary>
    /// Service interface to the hft internal service.
    /// </summary>
    [PublicAPI]
    public interface IHftInternalServiceApi
    {
        /// <summary>
        /// Determines whether this hft internal service is alive.
        /// </summary>
        [Get("/api/IsAlive")]
        Task<IsAliveModel> IsAlive();
    }
}