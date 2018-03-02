using System;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Chaos;
using Lykke.Cqrs;
using Lykke.Service.HftInternalService.Core.Domain;
using Lykke.Service.HftInternalService.Services.Events;

namespace Lykke.Service.HftInternalService.Services.Handlers
{
    public class ApiKeyHandler
    {
        private readonly ILog _log;
        private readonly IChaosKitty _chaosKitty;
        private readonly IRepository<ApiKey> _apiKeyRepository;

        public ApiKeyHandler(
            [NotNull] ILog log,
            [NotNull] IChaosKitty chaosKitty,
            [NotNull] IRepository<ApiKey> apiKeyRepository)
        {
            _log = log.CreateComponentScope(nameof(ApiKeyHandler));
            _chaosKitty = chaosKitty ?? throw new ArgumentNullException(nameof(chaosKitty));
            _apiKeyRepository = apiKeyRepository ?? throw new ArgumentNullException(nameof(apiKeyRepository));
        }

        public async Task<CommandHandlingResult> Handle(Commands.CreateApiKeyCommand command, IEventPublisher eventPublisher)
        {
            _log.WriteInfo(nameof(Commands.CreateApiKeyCommand), command, "");

            var existedApiKey = await _apiKeyRepository.Get(x => x.WalletId == command.WalletId && x.ValidTill == null);
            if (existedApiKey != null)
            {
                existedApiKey.ValidTill = DateTime.UtcNow;
                await _apiKeyRepository.Update(existedApiKey);
                _chaosKitty.Meow("repository unavailable");

                eventPublisher.PublishEvent(new ApiKeyUpdatedEvent { ApiKey = existedApiKey.Id.ToString(), WalletId = existedApiKey.WalletId, Enabled = false });
            }

            var key = new ApiKey { Id = Guid.Parse(command.ApiKey), ClientId = command.ClientId, WalletId = command.WalletId };
            await _apiKeyRepository.Add(key);
            _chaosKitty.Meow("repository unavailable");

            eventPublisher.PublishEvent(new ApiKeyUpdatedEvent { ApiKey = key.Id.ToString(), WalletId = key.WalletId, Enabled = true });

            return CommandHandlingResult.Ok();
        }

        public async Task<CommandHandlingResult> Handle(Commands.DeleteApiKeyCommand command, IEventPublisher eventPublisher)
        {
            _log.WriteInfo(nameof(Commands.DeleteApiKeyCommand), command, "");

            var existedApiKey = await _apiKeyRepository.Get(Guid.Parse(command.ApiKey));
            if (existedApiKey != null)
            {
                existedApiKey.ValidTill = DateTime.UtcNow;
                await _apiKeyRepository.Update(existedApiKey);
                _chaosKitty.Meow("repository unavailable");

                eventPublisher.PublishEvent(new ApiKeyUpdatedEvent { ApiKey = command.ApiKey, WalletId = existedApiKey.WalletId, Enabled = false });
            }

            return CommandHandlingResult.Ok();
        }


    }
}
