using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.HftInternalService.Core.Domain;

namespace Lykke.Service.HftInternalService.Core.Services
{
    public interface IApiKeyService
    {
        Task<ApiKey> GenerateApiKeyAsync(string clientId, string walletId, string walletName = null);
        Task DeleteApiKeyAsync(ApiKey key);
        Task<ApiKey[]> GetApiKeysAsync(string clientId, bool hideKeys);
        Task<ApiKey> GetApiKeyAsync(string id);
        string GenerateJwtToken(string clientId, string walletId, string walletName);
        Task SetTokensAsync();
        Task<IReadOnlyCollection<ApiKey>> GetValidKeys();
    }
}
