using System.ComponentModel.DataAnnotations;

namespace Lykke.Service.HftInternalService.Models
{
    public class CreateApiKeyRequest
    {
        [Required]
        public string ClientId { get; set; }
        public string Name { get; set; }
    }
}
