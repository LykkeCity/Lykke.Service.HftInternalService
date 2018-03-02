using System;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Cqrs;
using Lykke.Service.HftInternalService.Core.Domain;
using Lykke.Service.HftInternalService.Core.Services;
using Lykke.Service.HftInternalService.Services.Commands;

namespace Lykke.Service.HftInternalService.Services
{
    public class ApiKeyService : IApiKeyService
    {
        private readonly ICqrsEngine _cqrsEngine;
        private readonly IRepository<ApiKey> _apiKeyRepository;

        public ApiKeyService([NotNull] IRepository<ApiKey> orderStateRepository,
            [NotNull] ICqrsEngine cqrsEngine)
        {
            _apiKeyRepository = orderStateRepository ?? throw new ArgumentNullException(nameof(orderStateRepository));
            _cqrsEngine = cqrsEngine ?? throw new ArgumentNullException(nameof(cqrsEngine));
        }

        public Task<ApiKey> GenerateApiKeyAsync(string clientId, string walletId)
        {
            var apiKey = Guid.NewGuid();
            var key = new ApiKey { Id = apiKey, ClientId = clientId, WalletId = walletId };

            _cqrsEngine.SendCommand(
                new CreateApiKeyCommand { ApiKey = key.Id.ToString(), ClientId = clientId, WalletId = walletId },
                "api-key", "api-key");

            return Task.FromResult(key);
        }

        public Task DeleteApiKeyAsync(ApiKey key)
        {
            _cqrsEngine.SendCommand(
                new DisableApiKeyCommand { ApiKey = key.Id.ToString() },
                "api-key", "api-key");

            return Task.CompletedTask;
        }

        public async Task<ApiKey[]> GetApiKeysAsync(string clientId)
        {
            var existedApiKeys = _apiKeyRepository.FilterBy(x => x.ClientId == clientId && x.ValidTill == null).ToArray(); // todo: make async calls
            return existedApiKeys;
        }

        public async Task<ApiKey> GetApiKeyAsync(string id)
        {
            if (Guid.TryParse(id, out Guid key))
            {
                return await _apiKeyRepository.Get(key);
            }
            return null;
        }
    }
}
