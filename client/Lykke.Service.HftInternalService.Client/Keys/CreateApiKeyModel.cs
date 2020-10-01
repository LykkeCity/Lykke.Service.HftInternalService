using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using Lykke.Service.HftInternalService.Client.Api;

namespace Lykke.Service.HftInternalService.Client.Keys
{
    /// <summary>
    /// Request class for <see cref="IKeysApi.CreateKey"/> call.
    /// </summary>
    [PublicAPI]
    public class CreateApiKeyModel
    {
        /// <summary>
        /// The client identifier.
        /// </summary>
        [Required]
        public string ClientId { get; set; }

        /// <summary>
        /// The name for the new api wallet.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description for the api wallet.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Is the api key for api v2 only
        /// </summary>
        public bool Apiv2Only { get; set; }
    }
}
