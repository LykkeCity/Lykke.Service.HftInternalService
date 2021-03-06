﻿using System.Threading.Tasks;
using Lykke.Service.HftInternalService.Core.Domain;

namespace Lykke.Service.HftInternalService.Core.Services
{
    public interface IWalletService
    {
        Task<ApiKey> CreateWallet(string clientId, bool apiv2Only = false, string name = null, string description = null);
        Task DeleteWallet(ApiKey key);
    }
}
