using System.Threading.Tasks;
using Lykke.Service.HftInternalService.Core.Domain;

namespace Lykke.Service.HftInternalService.Core.Services
{

    public interface IApiKeyService
    {
        Task<ApiKey> GenerateApiKeyAsync(string clientId, string accountId);
        Task DeleteApiKeyAsync(ApiKey key);
        Task<ApiKey[]> GetApiKeysAsync(string clientId);
        Task<ApiKey> GetApiKeyAsync(string id);
    }
}
