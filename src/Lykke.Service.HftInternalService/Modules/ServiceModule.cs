using System;
using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.Service.ClientAccount.Client.AutorestClient;
using Lykke.Service.HftInternalService.Core;
using Lykke.Service.HftInternalService.Core.Domain;
using Lykke.Service.HftInternalService.Core.Services;
using Lykke.Service.HftInternalService.MongoRepositories;
using Lykke.Service.HftInternalService.Services;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace Lykke.Service.HftInternalService.Modules
{
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<AppSettings> _settings;
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public ServiceModule(IReloadingManager<AppSettings> settings, ILog log)
        {
            _settings = settings;
            _log = log;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            // TODO: Do not register entire settings in container, pass necessary settings to services which requires them
            // ex:
            //  builder.RegisterType<QuotesPublisher>()
            //      .As<IQuotesPublisher>()
            //      .WithParameter(TypedParameter.From(_settings.CurrentValue.QuotesPublication))

            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterInstance(_settings.CurrentValue.HighFrequencyTradingService.CacheSettings)
                .SingleInstance();

            RegisterApiKeyService(builder);

            BindMongoDb(builder);
            BindRedis(builder);

            builder.Populate(_services);
        }


        private void BindRedis(ContainerBuilder builder)
        {
            var apiKeysRedisCache = new RedisCache(new RedisCacheOptions
            {
                Configuration = _settings.CurrentValue.HighFrequencyTradingService.CacheSettings.RedisConfiguration,
                InstanceName = _settings.CurrentValue.HighFrequencyTradingService.CacheSettings.ApiKeyCacheInstance
            });
            builder.RegisterInstance(apiKeysRedisCache)
                .As<IDistributedCache>()
                .Keyed<IDistributedCache>("apiKeys")
                .SingleInstance();
        }

        private void RegisterApiKeyService(ContainerBuilder builder)
        {
            builder.RegisterType<ApiKeyService>()
                .WithParameter(
                    new ResolvedParameter(
                        (pi, ctx) => pi.ParameterType == typeof(IDistributedCache),
                        (pi, ctx) => ctx.ResolveKeyed<IDistributedCache>("apiKeys")))
                .As<IApiKeyService>()
                .SingleInstance();


            builder.RegisterType<MongoRepository<ApiKey>>()
                .As<IRepository<ApiKey>>()
                .SingleInstance();

            builder.RegisterType<WalletService>()
                .As<IWalletService>()
                .SingleInstance();

            builder.RegisterType<ClientAccountService>()
                .As<IClientAccountService>()
                .WithParameter(new TypedParameter(typeof(Uri), new Uri(_settings.CurrentValue.HftInternalService.ClientAccountServiceApiUrl)));
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
