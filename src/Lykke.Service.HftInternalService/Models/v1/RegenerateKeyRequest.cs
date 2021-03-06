﻿using System.ComponentModel.DataAnnotations;

namespace Lykke.Service.HftInternalService.Models.v1
{
    /// <summary>
    /// v1 Request model for regenerating api keys
    /// </summary>
    public class RegenerateKeyRequest
    {
        /// <summary>The client id</summary>
        [Required]
        public string ClientId { get; set; }

        /// <summary>The wallet id</summary>
        [Required]
        public string WalletId { get; set; }
    }
}
