using JetBrains.Annotations;

namespace Lykke.Service.HftInternalService.Client.Keys
{
    /// <summary>
    /// Model for an api-key of a high-frequency trading wallet.
    /// </summary>
    [PublicAPI]
    public class ApiKeyModel
    {
        /// <summary>ID of the key</summary>
        public string Id { get; set; }

        /// <summary>
        /// The API key of the wallet.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// The wallet identifier.
        /// </summary>
        public string WalletId { get; set; }

        /// <summary>
        /// The client identifier.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Indicating whether this api-key is enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Is the api key for api v2 only
        /// </summary>
        public bool Apiv2Only { get; set; }
    }
}
