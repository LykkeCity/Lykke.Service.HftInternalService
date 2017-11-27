using Autofac;
using Lykke.Service.ClientAccount.Client;
using Lykke.Service.HftInternalService.Core;
using Lykke.SettingsReader;

namespace Lykke.Service.HftInternalService.Modules
{
    public class ClientsModule : Module
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
