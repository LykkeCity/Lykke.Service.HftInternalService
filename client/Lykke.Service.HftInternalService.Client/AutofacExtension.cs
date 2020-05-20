using System;
using Autofac;
using JetBrains.Annotations;
using Lykke.HttpClientGenerator;
using Lykke.HttpClientGenerator.Infrastructure;

namespace Lykke.Service.HftInternalService.Client
{
    /// <summary>
    /// Extension for client registration
    /// </summary>
    [PublicAPI]
    public static class AutofacExtension
    {
        /// <summary>
        /// Registers <see cref="IHftInternalClient"/> in Autofac container.
        /// </summary>
        /// <param name="builder">Autofac container builder.</param>
        /// <param name="serviceUrl">Hft internal service url.</param>
        /// <param name="builderConfigure">Optional <see cref="HttpClientGeneratorBuilder"/> configure handler.</param>
        public static void RegisterHftInternalClient(
            [NotNull] this ContainerBuilder builder,
            [NotNull] string serviceUrl,
            [CanBeNull] Func<HttpClientGeneratorBuilder, HttpClientGeneratorBuilder> builderConfigure = null)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            if (serviceUrl == null)
                throw new ArgumentNullException(nameof(serviceUrl));

            var clientBuilder = HttpClientGenerator.HttpClientGenerator.BuildForUrl(serviceUrl)
                .WithAdditionalCallsWrapper(new ExceptionHandlerCallsWrapper());

            clientBuilder = builderConfigure?.Invoke(clientBuilder) ?? clientBuilder.WithoutRetries();

            builder.RegisterInstance(new HftInternalClient(clientBuilder.Create()))
                .As<IHftInternalClient>()
                .SingleInstance();
        }
    }
}
