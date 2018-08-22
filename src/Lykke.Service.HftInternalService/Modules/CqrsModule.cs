using System.Collections.Generic;
using Autofac;
using JetBrains.Annotations;
using Lykke.Common.Chaos;
using Lykke.Common.Log;
using Lykke.Cqrs;
using Lykke.Cqrs.Configuration;
using Lykke.Messaging;
using Lykke.Messaging.RabbitMq;
using Lykke.Messaging.Serialization;
using Lykke.Service.HftInternalService.Core;
using Lykke.Service.HftInternalService.Services.Commands;
using Lykke.Service.HftInternalService.Services.Events;
using Lykke.Service.HftInternalService.Services.Handlers;
using Lykke.SettingsReader;

namespace Lykke.Service.HftInternalService.Modules
{
    [UsedImplicitly]
    internal sealed class CqrsModule : Module
    {
        private readonly HftInternalServiceSettings _settings;

        public CqrsModule(IReloadingManager<AppSettings> settingsManager)
        {
            _settings = settingsManager.CurrentValue.HftInternalService;
        }

        protected override void Load(ContainerBuilder builder)
        {
            if (_settings.ChaosKitty != null)
            {
                builder
                    .RegisterType<ChaosKitty>()
                    .WithParameter(TypedParameter.From(_settings.ChaosKitty.StateOfChaos))
                    .As<IChaosKitty>()
                    .SingleInstance();
            }
            else
            {
                builder
                    .RegisterType<SilentChaosKitty>()
                    .As<IChaosKitty>()
                    .SingleInstance();
            }

            MessagePackSerializerFactory.Defaults.FormatterResolver =
                MessagePack.Resolvers.ContractlessStandardResolver.Instance;

            builder
                .Register(context => new AutofacDependencyResolver(context))
                .As<IDependencyResolver>()
                .SingleInstance();

            var rabbitMqSettings = new RabbitMQ.Client.ConnectionFactory {Uri = _settings.SagasRabbitMqConnStr};

            builder.RegisterType<ApiKeyHandler>();

            builder.Register(ctx =>
            {
                var logFactory = ctx.Resolve<ILogFactory>();
#if DEBUG
                var broker = rabbitMqSettings.Endpoint + "/debug";
#else
                var broker = rabbitMqSettings.Endpoint.ToString();
#endif
                var messagingEngine = new MessagingEngine(logFactory,
                    new TransportResolver(new Dictionary<string, TransportInfo>
                    {
                        {
                            "RabbitMq",
                            new TransportInfo(broker, rabbitMqSettings.UserName, rabbitMqSettings.Password, "None",
                                "RabbitMq")
                        }
                    }),
                    new RabbitMqTransportFactory(logFactory));

                var defaultRetryDelay = (long) _settings.RetryDelay.TotalMilliseconds;

                const string defaultPipeline = "commands";
                const string defaultRoute = "self";

                return new CqrsEngine(logFactory,
                    ctx.Resolve<IDependencyResolver>(),
                    messagingEngine,
                    new DefaultEndpointProvider(),
                    true,
                    Register.DefaultEndpointResolver(new RabbitMqConventionEndpointResolver(
                        "RabbitMq",
                        SerializationFormat.MessagePack,
                        environment: "lykke",
                        exclusiveQueuePostfix: _settings.QueuePostfix)),

                    Register.BoundedContext("api-key")
                        .FailedCommandRetryDelay(defaultRetryDelay)
                        .ListeningCommands(
                            typeof(CreateApiKeyCommand),
                            typeof(DisableApiKeyCommand))
                        .On(defaultRoute)
                        .PublishingEvents(
                            typeof(ApiKeyUpdatedEvent))
                        .With(defaultPipeline)
                        .WithCommandsHandler<ApiKeyHandler>(),

                    Register.DefaultRouting
                        .PublishingCommands(
                            typeof(CreateApiKeyCommand),
                            typeof(DisableApiKeyCommand))
                        .To("api-key").With(defaultPipeline)
                    );
                })
                .As<ICqrsEngine>().SingleInstance();
        }
    }
}
