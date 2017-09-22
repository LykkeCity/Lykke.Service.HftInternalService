using System.ComponentModel.DataAnnotations;

namespace Lykke.Service.HftInternalService.Models
{
    public class CreateAccountRequest
    {
        [Required]
        public string ClientId { get; set; }
        [Required]
        public string AdminApiKey { get; set; }
    }
}
