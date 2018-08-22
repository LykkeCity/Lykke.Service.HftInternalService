using System;
using JetBrains.Annotations;
using Lykke.Sdk;
using Lykke.Service.HftInternalService.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.HftInternalService
{
    internal sealed class Startup
    {
        private readonly LykkeSwaggerOptions _swaggerOptions = new LykkeSwaggerOptions
        {
            ApiVersion = "v1",
            ApiTitle = "HftInternalService API"
        };

        public IHostingEnvironment Environment { get; }

        public Startup(IHostingEnvironment env)
        {
            Environment = env;
        }

        [UsedImplicitly]
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.BuildServiceProvider<AppSettings>(options =>
            {
                options.Logs = logs =>
                {
                    logs.AzureTableName = "HftInternalServiceLog";
                    logs.AzureTableConnectionStringResolver =
                        settings => settings.HftInternalService.Db.LogsConnString;
                };

                options.SwaggerOptions = _swaggerOptions;

                options.Swagger = swagger => swagger.CustomSchemaIds(x => x.FullName);
            });
        }

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
        {
            app.UseLykkeConfiguration(options => options.SwaggerOptions = _swaggerOptions);
        }
    }
}
