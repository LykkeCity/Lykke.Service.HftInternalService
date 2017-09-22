using System;
using System.Threading.Tasks;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Lykke.Service.HftInternalService.Core.Domain;
using Lykke.Service.HftInternalService.Core.Services;

namespace Lykke.Service.HftInternalService.Services
{
    public class TrustedAccountService : ITrustedAccountService
    {
        private readonly IApiKeyGenerator _apiKeyGenerator;
        private readonly IMatchingEngineClient _matchingEngineClient;
        public TrustedAccountService(IApiKeyGenerator apiKeyGenerator, IMatchingEngineClient matchingEngineClient)
        {
            _matchingEngineClient = matchingEngineClient ?? throw new ArgumentNullException(nameof(matchingEngineClient));
            _apiKeyGenerator = apiKeyGenerator ?? throw new ArgumentNullException(nameof(apiKeyGenerator));
        }

        public async Task<TrustedAccount> CreateAccount(string clientId)
        {
            var apiKey = await _apiKeyGenerator.GenerateApiKeyAsync(clientId);
            return new TrustedAccount { ApiKey = apiKey, Id = Guid.Empty };
        }

        public async Task<TrustedAccount> GetAccount(string accountId)
        {
            throw new NotImplementedException();
        }

        public async Task GetBalances(string accountId)
        {
            throw new NotImplementedException();
        }

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
