using System.Threading.Tasks;

namespace Lykke.Service.HftInternalService.Core.Services
{

    public interface IApiKeyGenerator
    {
        Task<string> GenerateApiKeyAsync(string clientId);
    }
}
