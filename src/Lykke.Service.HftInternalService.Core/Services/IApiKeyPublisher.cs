using System.Threading.Tasks;
using Autofac;
using Common;
using Lykke.Service.HftInternalService.Contracts.Events;

namespace Lykke.Service.HftInternalService.Core.Services
{
    public interface IApiKeyPublisher : IStartable, IStopable
    {
        Task PublishAsync(ApiKeyUpdatedMessage message);
    }
}
