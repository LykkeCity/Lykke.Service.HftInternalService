using System.ComponentModel.DataAnnotations;

namespace Lykke.Service.HftInternalService.Models.v1
{
    /// <summary>
    /// v1 Request model for creating api keys
    /// </summary>
    public class CreateApiKeyRequest
    {
        /// <summary>The client id</summary>
        [Required]
        public string ClientId { get; set; }

        /// <summary>The api key name</summary>
        public string Name { get; set; }

        /// <summary>The api key description</summary>
        public string Description { get; set; }
    }
}
