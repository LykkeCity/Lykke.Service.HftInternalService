using System.ComponentModel.DataAnnotations;

namespace Lykke.Service.HftInternalService.Models.V2
{
    public class RegenerateKeyRequest
    {
        [Required]
        public string ClientId { get; set; }
        [Required]
        public string WalletId { get; set; }
    }
}
