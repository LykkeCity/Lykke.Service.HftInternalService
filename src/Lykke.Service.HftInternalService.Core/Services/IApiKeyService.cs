using System.Threading.Tasks;
using Lykke.Service.HftInternalService.Core.Domain;

namespace Lykke.Service.HftInternalService.Core.Services
{

    public interface IApiKeyService
    {
        Task<ApiKey> GenerateApiKeyAsync(string accountId, string keyName = null);
        Task<ApiKey[]> GetApiKeysAsync(string clientId);
    }
}
