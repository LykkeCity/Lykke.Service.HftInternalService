using System;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.HftInternalService.Core;
using Lykke.Service.HftInternalService.Core.Domain;
using Lykke.Service.HftInternalService.Core.Services;
using Microsoft.Extensions.Caching.Distributed;

namespace Lykke.Service.HftInternalService.Services
{
    public class ApiKeyService : IApiKeyService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly CacheSettings _cacheSettings;
        private readonly IRepository<ApiKey> _apiKeyRepository;

        public ApiKeyService(IDistributedCache distributedCache, CacheSettings cacheSettings, IRepository<ApiKey> orderStateRepository)
        {
            _cacheSettings = cacheSettings ?? throw new ArgumentNullException(nameof(cacheSettings));
            _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
            _apiKeyRepository = orderStateRepository ?? throw new ArgumentNullException(nameof(orderStateRepository));
        }

        public async Task<ApiKey> GenerateApiKeyAsync(string clientId, string walletId)
        {
            var apiKey = Guid.NewGuid();
            var apiKeyAsString = apiKey.ToString();
            await _distributedCache.SetStringAsync(GetCacheKey(apiKeyAsString), walletId);
            var existedApiKey = await _apiKeyRepository.Get(x => x.WalletId == walletId && x.ValidTill == null);
            if (existedApiKey != null)
            {
                await _distributedCache.RemoveAsync(GetCacheKey(existedApiKey.Id.ToString()));
                existedApiKey.ValidTill = DateTime.UtcNow;
                await _apiKeyRepository.Update(existedApiKey);
            }
            var key = new ApiKey { Id = apiKey, ClientId = clientId, WalletId = walletId };
            await _apiKeyRepository.Add(key);

            return key;
        }

        public async Task DeleteApiKeyAsync(ApiKey key)
        {
            var existedApiKey = await _apiKeyRepository.Get(key.Id);
            if (existedApiKey != null)
            {
                await _distributedCache.RemoveAsync(GetCacheKey(existedApiKey.Id.ToString()));
                existedApiKey.ValidTill = DateTime.UtcNow;
                await _apiKeyRepository.Update(existedApiKey);
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

        private string GetCacheKey(string apiKey)
        {
            return _cacheSettings.GetApiKey(apiKey);
        }
    }
}
