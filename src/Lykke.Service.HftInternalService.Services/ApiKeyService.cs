using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Lykke.Cqrs;
using Lykke.Service.ClientAccount.Client.AutorestClient;
using Lykke.Service.HftInternalService.Core.Domain;
using Lykke.Service.HftInternalService.Core.Services;
using Lykke.Service.HftInternalService.Services.Commands;
using Microsoft.IdentityModel.Tokens;

namespace Lykke.Service.HftInternalService.Services
{
    public class ApiKeyService : IApiKeyService
    {
        private readonly ICqrsEngine _cqrsEngine;
        private readonly string _jwtSecret;
        private readonly string _jwtAud;
        private readonly IRepository<ApiKey> _apiKeyRepository;
        private readonly IClientAccountService _clientAccountService;

        public ApiKeyService(
            string jwtSecret,
            string jwtAud,
            IRepository<ApiKey> orderStateRepository,
            IClientAccountService clientAccountService,
            ICqrsEngine cqrsEngine)
        {
            _jwtSecret = jwtSecret;
            _jwtAud = jwtAud;
            _apiKeyRepository = orderStateRepository ?? throw new ArgumentNullException(nameof(orderStateRepository));
            _clientAccountService = clientAccountService;
            _cqrsEngine = cqrsEngine ?? throw new ArgumentNullException(nameof(cqrsEngine));
        }

        public Task<ApiKey> GenerateApiKeyAsync(string clientId, string walletId, string walletName = null)
        {
            var id = Guid.NewGuid();
            var token = GenerateJwtToken(clientId, walletId, walletName);
            var key = new ApiKey { Id = id, Token = token, ClientId = clientId, WalletId = walletId, Created = DateTime.UtcNow};

            _cqrsEngine.SendCommand(
                new CreateApiKeyCommand { ApiKey = key.Id.ToString(), Token = token, ClientId = clientId, WalletId = walletId, Created = DateTime.UtcNow },
                "api-key", "api-key");

            return Task.FromResult(key);
        }

        public Task DeleteApiKeyAsync(ApiKey key)
        {
            _cqrsEngine.SendCommand(
                new DisableApiKeyCommand { ApiKey = key.Id.ToString() },
                "api-key", "api-key");

            return Task.CompletedTask;
        }

        public Task<ApiKey[]> GetApiKeysAsync(string clientId, bool hideKeys)
        {
            var existedApiKeys = _apiKeyRepository
                .FilterBy(x => x.ClientId == clientId && x.ValidTill == null)
                .ToArray();

            if (hideKeys)
            {
                var now = DateTime.UtcNow;

                foreach (var key in existedApiKeys)
                {
                    if (!key.Created.HasValue || now - key.Created.Value >= TimeSpan.FromMinutes(5))
                    {
                        key.Id = Guid.Empty;
                        key.Token = null;
                    }
                }
            }

            return Task.FromResult(existedApiKeys);
        }

        public async Task<ApiKey> GetApiKeyAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            if (Guid.TryParse(id, out var key))
            {
                return await _apiKeyRepository.Get(key);
            }

            return _apiKeyRepository.FilterBy(x => x.Token == id).FirstOrDefault();
        }

        public Task SetTokensAsync()
        {
            _cqrsEngine.SendCommand(new SetTokensCommand(),
                "api-key", "api-key");

            return Task.CompletedTask;
        }

        public string GenerateJwtToken(string clientId, string walletId, string walletName)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, walletName ?? "wallet"),
                    new Claim(JwtRegisteredClaimNames.Aud, _jwtAud),
                    new Claim("client-id", clientId),
                    new Claim("wallet-id", walletId)
                }),
                Expires = DateTime.UtcNow.AddYears(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
