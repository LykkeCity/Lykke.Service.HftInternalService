using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Chaos;
using Lykke.Common.Log;
using Lykke.Cqrs;
using Lykke.Service.HftInternalService.Core.Domain;
using Lykke.Service.HftInternalService.Core.Services;
using Lykke.Service.HftInternalService.Services.Events;

namespace Lykke.Service.HftInternalService.Services.Handlers
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class ApiKeyHandler
    {
        private readonly ILog _log;
        private readonly IChaosKitty _chaosKitty;
        private readonly IRepository<ApiKey> _apiKeyRepository;
        public IApiKeyService ApiKeyService { get; set; }

        public ApiKeyHandler(
            [NotNull] ILogFactory logFactory,
            [NotNull] IChaosKitty chaosKitty,
            [NotNull] IRepository<ApiKey> apiKeyRepository
            )
        {
            _log = logFactory.CreateLog(this);
            _chaosKitty = chaosKitty ?? throw new ArgumentNullException(nameof(chaosKitty));
            _apiKeyRepository = apiKeyRepository ?? throw new ArgumentNullException(nameof(apiKeyRepository));
        }

        public async Task<CommandHandlingResult> Handle(Commands.CreateApiKeyCommand command, IEventPublisher eventPublisher)
        {
            _log.Info("Create api key.", command);

            var existedApiKey = await _apiKeyRepository.Get(x => x.WalletId == command.WalletId && x.ValidTill == null);
            if (existedApiKey != null)
            {
                existedApiKey.ValidTill = DateTime.UtcNow;

                if (string.IsNullOrEmpty(existedApiKey.Token))
                    existedApiKey.Token = command.Token;

                await _apiKeyRepository.Update(existedApiKey);

                eventPublisher.PublishEvent(new ApiKeyUpdatedEvent { ApiKey = existedApiKey.Token ?? existedApiKey.Id.ToString(), Token = command.Token, WalletId = existedApiKey.WalletId, Enabled = false });
            }

            var key = new ApiKey { Id = Guid.Parse(command.ApiKey), Token = command.Token, ClientId = command.ClientId, WalletId = command.WalletId, Created = command.Created};
            await _apiKeyRepository.Add(key);
            _chaosKitty.Meow("repository unavailable");

            eventPublisher.PublishEvent(new ApiKeyUpdatedEvent { ApiKey = key.Token ?? key.Id.ToString(), Token = command.Token, WalletId = key.WalletId, Enabled = true });

            return CommandHandlingResult.Ok();
        }

        public async Task<CommandHandlingResult> Handle(Commands.DisableApiKeyCommand command, IEventPublisher eventPublisher)
        {
            _log.Info("Disable api key.", command);

            var existedApiKey = await _apiKeyRepository.Get(Guid.Parse(command.ApiKey));
            if (existedApiKey != null)
            {
                existedApiKey.ValidTill = DateTime.UtcNow;
                await _apiKeyRepository.Update(existedApiKey);
                _chaosKitty.Meow("repository unavailable");

                eventPublisher.PublishEvent(new ApiKeyUpdatedEvent { ApiKey = existedApiKey.Token ?? command.ApiKey, WalletId = existedApiKey.WalletId, Enabled = false });
            }

            return CommandHandlingResult.Ok();
        }

        public async Task<CommandHandlingResult> Handle(Commands.SetTokensCommand command, IEventPublisher eventPublisher)
        {
            _log.Info("Set tokens", command);

            var noTokenKeys = _apiKeyRepository.FilterBy(x => string.IsNullOrEmpty(x.Token));

            var tasks = new List<Task>();

            foreach (var key in noTokenKeys)
            {
                key.Token = ApiKeyService.GenerateJwtToken(key.ClientId, key.WalletId, null);
                tasks.Add(_apiKeyRepository.Update(key));
                eventPublisher.PublishEvent(new ApiKeyUpdatedEvent {
                    ApiKey = key.Token ?? key.Id.ToString(),
                    Token = key.Token,
                    WalletId = key.WalletId,
                    Enabled = key.ValidTill == null || key.ValidTill > DateTime.UtcNow
                });
            }

            _log.Info($"Waiting for {tasks.Count} tokens to be added...");
            await Task.WhenAll(tasks);
            _log.Info("Tokens were assigned");
            return CommandHandlingResult.Ok();
        }
    }
}
