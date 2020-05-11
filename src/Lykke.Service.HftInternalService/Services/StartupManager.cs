using System.Threading.Tasks;
using Lykke.Cqrs;
using Lykke.Sdk;

namespace Lykke.Service.HftInternalService.Services
{
    public class StartupManager : IStartupManager
    {
        private readonly ICqrsEngine _cqrsEngine;

        public StartupManager(
            ICqrsEngine cqrsEngine
        )
        {
            _cqrsEngine = cqrsEngine;
        }

        public Task StartAsync()
        {
            _cqrsEngine.StartSubscribers();
            _cqrsEngine.StartProcesses();

            return Task.CompletedTask;
        }
    }
}
