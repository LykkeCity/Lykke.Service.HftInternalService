using System.Threading.Tasks;
using Lykke.Service.HftInternalService.Core.Domain;

namespace Lykke.Service.HftInternalService.Core.Services
{
    public interface IAccountService
    {
        Task<ApiKey> CreateAccount(string clientId, string name = null);
        Task DeleteAccount(ApiKey key);
        Task<string> CashInOut(string accountId, string assetId, double amount);
    }
}
