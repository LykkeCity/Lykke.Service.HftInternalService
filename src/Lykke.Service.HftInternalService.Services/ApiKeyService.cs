using System;
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
        private readonly HighFrequencyTradingSettings _settings;
        private readonly IRepository<ApiKey> _apiKeyRepository;

        public ApiKeyService(IDistributedCache distributedCache, HighFrequencyTradingSettings settings, IRepository<ApiKey> orderStateRepository)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
            _apiKeyRepository = orderStateRepository ?? throw new ArgumentNullException(nameof(orderStateRepository));
        }

        public async Task<string> GenerateApiKeyAsync(string clientId)
        {
            var apiKey = Guid.NewGuid();
            var apiKeyAsString = apiKey.ToString();
            await _distributedCache.SetStringAsync(GetCacheKey(apiKeyAsString), clientId);
            var existedApiKey = await _apiKeyRepository.Get(x => x.ClientId == clientId && x.ValidTill == null);
            if (existedApiKey != null)
            {
                await _distributedCache.RemoveAsync(GetCacheKey(existedApiKey.Id.ToString()));
                existedApiKey.ValidTill = DateTime.UtcNow;
                await _apiKeyRepository.Update(existedApiKey);
            }
            await _apiKeyRepository.Add(new ApiKey { Id = apiKey, ClientId = clientId });

            return apiKeyAsString;
        }

        public async Task<string> GetApiKeyAsync(string clientId)
        {
            var existedApiKey = await _apiKeyRepository.Get(x => x.ClientId == clientId && x.ValidTill == null);
            return existedApiKey?.Id.ToString();
        }

        private string GetCacheKey(string apiKey)
        {
            return _settings.CacheSettings.GetApiKey(apiKey);
        }
    }
}
