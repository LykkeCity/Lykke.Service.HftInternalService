using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.ClientAccount.Client.AutorestClient;
using Lykke.Service.HftInternalService.Client.Keys;
using Lykke.Service.HftInternalService.Core.Services;
using Lykke.Service.HftInternalService.Models.V2;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Lykke.Service.HftInternalService.Controllers.V2
{
    /// <summary>
    /// Api keys controller - Version 2
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/v2/[controller]")]
    [ApiController]
    public class KeysController : Controller
    {
        private readonly IWalletService _walletService;
        private readonly IApiKeyService _apiKeyService;
        private readonly IClientAccountService _clientAccountService;
        private readonly IMapper _mapper;
        private readonly ILog _log;

        /// <inheritdoc />
        public KeysController(IWalletService walletService, IApiKeyService apiKeyService, IClientAccountService clientAccountService,
            [NotNull] IMapper mapper, ILogFactory logFactory)
        {
            _log = logFactory.CreateLog(this);
            _apiKeyService = apiKeyService ?? throw new ArgumentNullException(nameof(apiKeyService));
            _walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
            _clientAccountService = clientAccountService ?? throw new ArgumentNullException(nameof(clientAccountService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Create api-key for a specified client.
        /// </summary>
        /// <param name="request">Key creation settings.</param>
        /// <returns>Wallet ID and API key.</returns>
        [HttpPost]
        [SwaggerOperation("CreateKey")]
        [ProducesResponseType(typeof(ApiKeyDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateKey([FromBody] CreateApiKeyRequest request)
        {
            if (request == null)
                return BadRequest();

            var client = await _clientAccountService.GetClientByIdAsync(request.ClientId);
            if (client == null)
            {
                ModelState.AddModelError("ClientId", request.ClientId);
                return BadRequest(ModelState);
            }

            var apiKey = await _walletService.CreateWallet(request.ClientId, request.Apiv2Only, request.Name, request.Description);
            return Ok(_mapper.Map<ApiKeyDto>(apiKey));
        }

        /// <summary>
        /// Creates new api-key for all existing wallets.
        /// </summary>
        [HttpPost("newAll")]
        [SwaggerOperation("RegenerateAllKeys")]
        [ProducesResponseType(typeof(ApiKeyDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RegenerateKey()
        {
            var allApiKeys = await _apiKeyService.GetValidKeys();

            _log.Info($"{allApiKeys.Count} API keys found. Regenerating them all...");

            foreach (var existingApiKey in allApiKeys)
            {
                await _apiKeyService.GenerateApiKeyAsync(existingApiKey.ClientId, existingApiKey.WalletId);
            }

            return Ok($"{allApiKeys.Count} API keys were scheduled for regeneration.");
        }

        /// <summary>
        /// Create new api-key for existing wallet.
        /// </summary>
        /// <param name="request">Client id and wallet id.</param>
        /// <returns>Wallet ID and API key.</returns>
        [HttpPost("new")]
        [SwaggerOperation("RegenerateKey")]
        [ProducesResponseType(typeof(ApiKeyDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RegenerateKey([FromBody] RegenerateKeyRequest request)
        {
            if (request == null)
                return BadRequest();

            var client = await _clientAccountService.GetClientByIdAsync(request.ClientId);
            if (client == null)
            {
                ModelState.AddModelError("ClientId", request.ClientId);
                return BadRequest(ModelState);
            }

            var wallet = await _clientAccountService.GetWalletAsync(request.WalletId);
            if (wallet == null)
            {
                ModelState.AddModelError("WalletId", request.WalletId);
                return BadRequest(ModelState);
            }

            var apiKey = await _apiKeyService.GenerateApiKeyAsync(request.ClientId, request.WalletId, request.Apiv2Only);
            return Ok(_mapper.Map<ApiKeyDto>(apiKey));
        }

        /// <summary>
        /// Get all api keys for a specified client.
        /// </summary>
        /// <param name="clientId"></param>
        [HttpGet]
        [SwaggerOperation("GetKeys")]
        [ProducesResponseType(typeof(ApiKeyDto[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetKeys(string clientId)
        {
            if (string.IsNullOrWhiteSpace(clientId))
                return BadRequest();

            var keys = await _apiKeyService.GetApiKeysAsync(clientId, true);
            return Ok(keys.Select(_mapper.Map<ApiKeyDto>));
        }

        /// <summary>
        /// Get api key.
        /// </summary>
        /// <param name="apiKey"></param>
        [HttpGet("{apiKey}")]
        [SwaggerOperation("GetKey")]
        [ProducesResponseType(typeof(ApiKeyDto[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetKey(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                return BadRequest();

            var key = await _apiKeyService.GetApiKeyAsync(apiKey);
            return Ok(_mapper.Map<ApiKeyDto>(key));
        }

        /// <summary>
        /// Delete specified api-key.
        /// </summary>
        /// <param name="key"></param>
        [HttpDelete("{key}")]
        [SwaggerOperation("DeleteKey")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return BadRequest();

            var apiKey = await _apiKeyService.GetApiKeyAsync(key);
            if (apiKey == null)
                return NotFound();

            await _walletService.DeleteWallet(apiKey);
            return Ok();
        }

        /// <summary>
        /// Set tokens
        /// </summary>
        [HttpPost("setTokens")]
        [SwaggerOperation("SetTokens")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SetTokens()
        {
            await _apiKeyService.SetTokensAsync();
            return Ok();
        }

        /// <summary>
        /// Get all api key ids
        /// </summary>
        [HttpGet("ids")]
        [SwaggerOperation("GetAllKeyIds")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAllKeyIds([FromQuery]ApiType type = ApiType.All)
        {
            var ids = await _apiKeyService.GetValidKeys();

            switch (type)
            {
                case ApiType.Apiv1:
                    ids = ids.Where(x => !x.Apiv2Only).ToList();
                    break;
                case ApiType.Apiv2:
                    ids = ids.Where(x => x.Apiv2Only).ToList();
                    break;
            }

            return Ok(ids.Select(x => x.Id.ToString()).ToList());
        }
    }
}
