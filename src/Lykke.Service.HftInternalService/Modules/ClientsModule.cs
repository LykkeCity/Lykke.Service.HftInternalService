using Autofac;
using JetBrains.Annotations;
using Lykke.Service.ClientAccount.Client;
using Lykke.Service.HftInternalService.Core;
using Lykke.SettingsReader;

namespace Lykke.Service.HftInternalService.Modules
{
    [UsedImplicitly]
    internal sealed class ClientsModule : Module
    {
        private readonly IReloadingManager<AppSettings> _settings;

        public ClientsModule(IReloadingManager<AppSettings> settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterLykkeServiceClient(_settings.CurrentValue.ClientAccountServiceClient.ServiceUrl);
        }
    }
}
