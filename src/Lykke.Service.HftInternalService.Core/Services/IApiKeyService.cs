using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.HftInternalService.Core.Domain;

namespace Lykke.Service.HftInternalService.Core.Services
{
    public interface IApiKeyService
    {
        Task<ApiKey> GenerateApiKeyAsync(string clientId, string walletId, bool apiv2Only = false, string walletName = null);
        Task DeleteApiKeyAsync(ApiKey key);
        Task<ApiKey[]> GetApiKeysAsync(string clientId, bool hideKeys);
        Task<ApiKey> GetApiKeyAsync(string id);
        string GenerateJwtToken(string apiKey, string clientId, string walletId, bool apiv2Only, string walletName);
        Task SetTokensAsync();
        Task<IReadOnlyCollection<ApiKey>> GetValidKeys();
    }
}
