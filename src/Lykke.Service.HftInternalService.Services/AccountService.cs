using System;
using System.Threading.Tasks;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Lykke.Service.ClientAccount.Client.AutorestClient;
using Lykke.Service.ClientAccount.Client.AutorestClient.Models;
using Lykke.Service.HftInternalService.Core.Domain;
using Lykke.Service.HftInternalService.Core.Services;

namespace Lykke.Service.HftInternalService.Services
{
    public class AccountService : IAccountService
    {
        private readonly IApiKeyService _apiKeyService;
        private readonly IMatchingEngineClient _matchingEngineClient;
        private readonly IClientAccountService _clientAccountService;
        public AccountService(IApiKeyService apiKeyService, IMatchingEngineClient matchingEngineClient, IClientAccountService clientAccountService)
        {
            _clientAccountService = clientAccountService ?? throw new ArgumentNullException(nameof(clientAccountService));
            _matchingEngineClient = matchingEngineClient ?? throw new ArgumentNullException(nameof(matchingEngineClient));
            _apiKeyService = apiKeyService ?? throw new ArgumentNullException(nameof(apiKeyService));
        }

        public async Task<ApiKey> CreateAccount(string clientId, string name = null)
        {
            var wallet = await _clientAccountService.CreateWalletAsync(new CreateWalletRequest(
                clientId: clientId,
                type: "HFT",
                name: name));
            var apiKey = await _apiKeyService.GenerateApiKeyAsync(clientId, wallet.Id);
            return apiKey;
        }

        public async Task DeleteAccount(ApiKey key)
        {
            await _clientAccountService.DeleteWalletAsync(key.AccountId);
            await _apiKeyService.DeleteApiKeyAsync(key);
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
