﻿using Autofac;
using JetBrains.Annotations;
using Lykke.Sdk;
using Lykke.Service.HftInternalService.Core;
using Lykke.Service.HftInternalService.Core.Domain;
using Lykke.Service.HftInternalService.Core.Services;
using Lykke.Service.HftInternalService.Services;
using Lykke.Service.HFTInternalService.MongoRepositories;
using Lykke.Service.HftInternalService.RabbitPublishers;
using Lykke.SettingsReader;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace Lykke.Service.HftInternalService.Modules
{
    [UsedImplicitly]
    internal sealed class ServiceModule : Module
    {
        private readonly IReloadingManager<AppSettings> _settings;

        public ServiceModule(IReloadingManager<AppSettings> settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            RegisterApiKeyService(builder);

            BindMongoDb(builder);
        }

        private void RegisterApiKeyService(ContainerBuilder builder)
        {
            builder.RegisterType<ApiKeyService>()
                .WithParameter("jwtSecret", _settings.CurrentValue.HftJwtAuth.JwtSecret)
                .WithParameter("jwtAud", _settings.CurrentValue.HftJwtAuth.JwtAud)
                .As<IApiKeyService>()
                .SingleInstance();

            builder.RegisterType<MongoRepository<ApiKey>>()
                .As<IRepository<ApiKey>>()
                .SingleInstance();

            builder.RegisterType<WalletService>()
                .As<IWalletService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>()
                .SingleInstance();

            builder.RegisterType<KeysPublisher>()
                .WithParameter(TypedParameter.From(_settings.CurrentValue.HftInternalService.Rabbit))
                .As<IKeysPublisher>()
                .As<IStartable>()
                .SingleInstance();
        }

        private void BindMongoDb(ContainerBuilder builder)
        {
            var mongoUrl = new MongoUrl(_settings.CurrentValue.HighFrequencyTradingService.MongoSettings.ConnectionString);
            ConventionRegistry.Register("Ignore extra", new ConventionPack { new IgnoreExtraElementsConvention(true) }, x => true);

            var database = new MongoClient(mongoUrl).GetDatabase(mongoUrl.DatabaseName);
            builder.RegisterInstance(database);
        }
    }
}
