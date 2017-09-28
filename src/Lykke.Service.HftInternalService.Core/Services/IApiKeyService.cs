using System.Threading.Tasks;

namespace Lykke.Service.HftInternalService.Core.Services
{

    public interface IApiKeyService
    {
        Task<string> GenerateApiKeyAsync(string clientId);
        Task<string> GetApiKeyAsync(string clientId);
    }
}
