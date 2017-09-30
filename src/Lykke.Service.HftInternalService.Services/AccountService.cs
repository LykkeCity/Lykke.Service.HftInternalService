﻿using System;
using System.Threading.Tasks;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Lykke.Service.HftInternalService.Core.Domain;
using Lykke.Service.HftInternalService.Core.Services;

namespace Lykke.Service.HftInternalService.Services
{
    public class AccountService : IAccountService
    {
        private readonly IApiKeyService _apiKeyService;
        private readonly IMatchingEngineClient _matchingEngineClient;
        public AccountService(IApiKeyService apiKeyService, IMatchingEngineClient matchingEngineClient)
        {
            _matchingEngineClient = matchingEngineClient ?? throw new ArgumentNullException(nameof(matchingEngineClient));
            _apiKeyService = apiKeyService ?? throw new ArgumentNullException(nameof(apiKeyService));
        }

        public async Task<Account> CreateAccount(string clientId)
        {
            var apiKey = await _apiKeyService.GenerateApiKeyAsync(clientId);
            // todo: use ClientAccount service for account creation
            return new Account { ApiKey = apiKey, Id = Guid.Empty, ClientId = clientId };
        }

        // todo: remove this method
        public async Task<string> CashInOut(string accountId, string assetId, double amount)
        {
            var id = GetNextRequestId();
            var response = await _matchingEngineClient.CashInOutAsync(id, accountId, assetId, amount);
            return $"{response.Status}: {response.Message}";
        }

        private string GetNextRequestId()
        {
            return Guid.NewGuid().ToString();
        }

    }
}
