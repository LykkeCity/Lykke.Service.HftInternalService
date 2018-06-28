using JetBrains.Annotations;
using Refit;

namespace Lykke.Service.HftInternalService.Client.Health
{
    /// <summary>
    /// Response class for <see cref="IHftInternalServiceApi.IsAlive"/> call.
    /// </summary>
    [PublicAPI]
    public class IsAliveModel
    {
        /// <summary>
        /// The name of the service.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The version of the service.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// The environment of the service (pod name).
        /// </summary>
        [AliasAs("Env")]
        public string Environment { get; set; }

        /// <summary>
        /// Indicating whether this instance is in debug or production mode.
        /// </summary>
        public bool IsDebug { get; set; }

        /// <summary>
        /// The possible health issues of the service.
        /// </summary>
        [AliasAs("IssueIndicators")]
        public IssueIndicator[] Issues { get; set; }
    }
}