using System.Threading.Tasks;
using Lykke.Service.HftInternalService.Core.Domain;

namespace Lykke.Service.HftInternalService.Core.Services
{
    public interface IAccountService
    {
        Task<ApiKey> CreateAccount(string clientId);
        Task DeleteAccount(string key);
        Task<string> CashInOut(string accountId, string assetId, double amount);
    }
}
