using System.ComponentModel.DataAnnotations;

namespace Lykke.Service.HftInternalService.Models.V2
{
    /// <summary>
    /// v2 Request model for regenerating api keys
    /// </summary>
    public class RegenerateKeyRequest
    {
        /// <summary>The client id</summary>
        [Required]
        public string ClientId { get; set; }

        /// <summary>The wallet id</summary>
        [Required]
        public string WalletId { get; set; }

        /// <summary>Is the api key for api v2 only</summary>
        public bool Apiv2Only { get; set; }
    }
}
