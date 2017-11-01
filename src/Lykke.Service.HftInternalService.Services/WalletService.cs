﻿using System;
using System.Threading.Tasks;
using Lykke.Service.ClientAccount.Client.AutorestClient;
using Lykke.Service.ClientAccount.Client.AutorestClient.Models;
using Lykke.Service.HftInternalService.Core.Domain;
using Lykke.Service.HftInternalService.Core.Services;

namespace Lykke.Service.HftInternalService.Services
{
    public class WalletService : IWalletService
    {
        private readonly IApiKeyService _apiKeyService;
        private readonly IClientAccountService _clientAccountService;
        public WalletService(IApiKeyService apiKeyService, IClientAccountService clientAccountService)
        {
            _clientAccountService = clientAccountService ?? throw new ArgumentNullException(nameof(clientAccountService));
            _apiKeyService = apiKeyService ?? throw new ArgumentNullException(nameof(apiKeyService));
        }

        public async Task<ApiKey> CreateWallet(string clientId, string name = null, string description = null)
        {
            var wallet = await _clientAccountService.CreateWalletAsync(new CreateWalletRequest(
                clientId: clientId,
                name: name,
                description: description));
            var apiKey = await _apiKeyService.GenerateApiKeyAsync(clientId, wallet.Id);
            return apiKey;
        }

        public async Task DeleteWallet(ApiKey key)
        {
            await _clientAccountService.DeleteWalletAsync(key.WalletId);
            await _apiKeyService.DeleteApiKeyAsync(key);
        }
    }
}
