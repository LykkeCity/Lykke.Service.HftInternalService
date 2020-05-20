using System;
using System.Threading.Tasks;
using Autofac;
using Lykke.Service.HftInternalService.Client.Messages;

namespace Lykke.Service.HftInternalService.Core.Services
{
    public interface IKeysPublisher : IStartable, IDisposable
    {
        Task PublishAsync(KeyUpdatedEvent message);
    }
}
