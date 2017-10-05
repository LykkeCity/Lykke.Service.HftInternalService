using System.Threading.Tasks;
using Lykke.Service.HftInternalService.Core.Domain;

namespace Lykke.Service.HftInternalService.Core.Services
{
    public interface IWalletService
    {
        Task<ApiKey> CreateWallet(string clientId, string name = null);
        Task DeleteWallet(ApiKey key);
    }
}
