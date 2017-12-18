using System;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.HftInternalService.Contracts.Events;
using Lykke.Service.HftInternalService.Core.Domain;
using Lykke.Service.HftInternalService.Core.Services;
using ApiKey = Lykke.Service.HftInternalService.Core.Domain.ApiKey;

namespace Lykke.Service.HftInternalService.Services
{
    public class ApiKeyService : IApiKeyService
    {
        private readonly IApiKeyPublisher _apiKeyPublisher;
        private readonly IRepository<ApiKey> _apiKeyRepository;

        public ApiKeyService([NotNull] IRepository<ApiKey> orderStateRepository, [NotNull] IApiKeyPublisher apiKeyPublisher)
        {
            _apiKeyPublisher = apiKeyPublisher ?? throw new ArgumentNullException(nameof(apiKeyPublisher));
            _apiKeyRepository = orderStateRepository ?? throw new ArgumentNullException(nameof(orderStateRepository));
        }

        public async Task<ApiKey> GenerateApiKeyAsync(string clientId, string walletId)
        {
            var apiKey = Guid.NewGuid();

            var existedApiKey = await _apiKeyRepository.Get(x => x.WalletId == walletId && x.ValidTill == null);
            if (existedApiKey != null)
            {
                existedApiKey.ValidTill = DateTime.UtcNow;
                await _apiKeyRepository.Update(existedApiKey);
                await PublishNotification(existedApiKey);
            }
            var key = new ApiKey { Id = apiKey, ClientId = clientId, WalletId = walletId };
            await _apiKeyRepository.Add(key);
            await PublishNotification(key);

            return key;
        }

        public async Task DeleteApiKeyAsync(ApiKey key)
        {
            var existedApiKey = await _apiKeyRepository.Get(key.Id);
            if (existedApiKey != null)
            {
                existedApiKey.ValidTill = DateTime.UtcNow;
                await _apiKeyRepository.Update(existedApiKey);
                await PublishNotification(existedApiKey);
            }
        }

        public async Task<ApiKey[]> GetApiKeysAsync(string clientId)
        {
            var existedApiKeys = _apiKeyRepository.FilterBy(x => x.ClientId == clientId && x.ValidTill == null).ToArray();
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

        private async Task PublishNotification(ApiKey apiKey)
        {
            var message = new ApiKeyUpdatedMessage { ApiKey = new Contracts.Events.ApiKey { Id = apiKey.Id, WalletId = apiKey.WalletId, Enabled = !apiKey.ValidTill.HasValue } };
            await _apiKeyPublisher.PublishAsync(message);
        }
    }
}
