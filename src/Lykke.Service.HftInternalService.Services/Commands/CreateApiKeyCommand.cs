﻿using System;
using MessagePack;

namespace Lykke.Service.HftInternalService.Services.Commands
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class CreateApiKeyCommand
    {
        public string ApiKey { get; set; }
        public string Token { get; set; }
        public string ClientId { get; set; }
        public string WalletId { get; set; }
        public DateTime? Created { get; set; }
        public bool Apiv2Only { get; set; }
    }
}
