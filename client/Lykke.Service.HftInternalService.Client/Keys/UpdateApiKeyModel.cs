using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

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
    }
}
