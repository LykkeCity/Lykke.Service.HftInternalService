﻿using System.ComponentModel.DataAnnotations;

namespace Lykke.Service.HftInternalService.Models.v1
{
    public class CreateApiKeyRequest
    {
        [Required]
        public string ClientId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
