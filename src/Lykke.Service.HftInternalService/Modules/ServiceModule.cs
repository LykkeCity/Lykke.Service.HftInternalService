using Autofac;
using Autofac.Core;
using Common.Log;
using Lykke.Service.HftInternalService.Core;
using Lykke.Service.HftInternalService.Core.Domain;
using Lykke.Service.HftInternalService.Core.Services;
using Lykke.Service.HftInternalService.MongoRepositories;
using Lykke.Service.HftInternalService.Services;
using Lykke.SettingsReader;
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

        public ServiceModule(IReloadingManager<AppSettings> settings, ILog log)
        {
            _settings = settings;
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            RegisterApiKeyService(builder);

            BindMongoDb(builder);
            BindRedis(builder);
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
                .WithParameter(TypedParameter.From(_settings.CurrentValue.HighFrequencyTradingService.CacheSettings))
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
