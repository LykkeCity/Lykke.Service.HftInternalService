using System.Threading.Tasks;
using Lykke.Service.HftInternalService.Core.Domain;

namespace Lykke.Service.HftInternalService.Core.Services
{
    public interface ITrustedAccountService
    {
        Task<TrustedAccount> CreateAccount(string clientId);
        Task<TrustedAccount> GetAccount(string accountId);
        Task GetBalances(string accountId);
        Task<string> CashInOut(string accountId, string assetId, double amount);
    }
}
