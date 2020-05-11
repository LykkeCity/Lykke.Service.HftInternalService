using System;
using System.Collections.Generic;
using Autofac;
using JetBrains.Annotations;
using Lykke.Common.Chaos;
using Lykke.Common.Log;
using Lykke.Cqrs;
using Lykke.Cqrs.Configuration;
using Lykke.Cqrs.Middleware.Logging;
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

            var rabbitMqSettings = new RabbitMQ.Client.ConnectionFactory {Uri = new Uri(_settings.SagasRabbitMqConnStr)};

            builder.RegisterType<ApiKeyHandler>()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

            builder.Register(ctx =>
            {
                var logFactory = ctx.Resolve<ILogFactory>();
                var broker = rabbitMqSettings.Endpoint.ToString();
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

                var engine = new CqrsEngine(logFactory,
                    ctx.Resolve<IDependencyResolver>(),
                    messagingEngine,
                    new DefaultEndpointProvider(),
                    true,
                    Register.DefaultEndpointResolver(new RabbitMqConventionEndpointResolver(
                        "RabbitMq",
                        SerializationFormat.MessagePack,
                        environment: "lykke",
                        exclusiveQueuePostfix: _settings.QueuePostfix)),

                    Register.EventInterceptors(new DefaultEventLoggingInterceptor(ctx.Resolve<ILogFactory>())),

                    Register.BoundedContext("api-key")
                        .FailedCommandRetryDelay(defaultRetryDelay)
                        .ListeningCommands(
                            typeof(CreateApiKeyCommand),
                            typeof(DisableApiKeyCommand),
                            typeof(SetTokensCommand)
                            )
                        .On(defaultRoute)
                        .PublishingEvents(
                            typeof(ApiKeyUpdatedEvent))
                        .With(defaultPipeline)
                        .WithCommandsHandler<ApiKeyHandler>(),

                    Register.DefaultRouting
                        .PublishingCommands(
                            typeof(CreateApiKeyCommand),
                            typeof(DisableApiKeyCommand),
                            typeof(SetTokensCommand)
                            )
                        .To("api-key").With(defaultPipeline)
                    );

                    engine.StartPublishers();
                    return engine;
                })
                .As<ICqrsEngine>()
                .AutoActivate()
                .SingleInstance();
        }
    }
}
