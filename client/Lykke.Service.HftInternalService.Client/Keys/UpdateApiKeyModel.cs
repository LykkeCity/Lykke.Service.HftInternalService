using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using Lykke.Service.HftInternalService.Client.Api;

namespace Lykke.Service.HftInternalService.Client.Keys
{
    /// <summary>
    /// Request class for <see cref="IKeysApi.UpdateKey"/> call.
    /// </summary>
    [PublicAPI]
    public class UpdateApiKeyModel
    {
        /// <summary>
        /// The client identifier.
        /// </summary>
        [Required]
        public string ClientId { get; set; }

        /// <summary>
        /// The id of the wallet to update.
        /// </summary>
        [Required]
        public string WalletId { get; set; }

        /// <summary>Is the api key for api v2 only</summary>
        public bool Apiv2Only { get; set; }
    }
}
