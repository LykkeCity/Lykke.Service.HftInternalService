using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lykke.Service.ClientAccount.Client.AutorestClient;
using Lykke.Service.HftInternalService.Core.Services;
using Lykke.Service.HftInternalService.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Service.HftInternalService.Controllers
{
    [Route("api/[controller]")]
    public class KeysController : Controller
    {
        private readonly IWalletService _walletService;
        private readonly IApiKeyService _apiKeyService;
        private readonly IClientAccountService _clientAccountService;

        public KeysController(IWalletService walletService, IApiKeyService apiKeyService, IClientAccountService clientAccountService)
        {
            _apiKeyService = apiKeyService ?? throw new ArgumentNullException(nameof(apiKeyService));
            _walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
            _clientAccountService = clientAccountService ?? throw new ArgumentNullException(nameof(clientAccountService));
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

            var apiKey = await _walletService.CreateWallet(request.ClientId, request.Name, request.Description);
            return Ok(new ApiKeyDto { Key = apiKey.Id.ToString(), Wallet = apiKey.WalletId });
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

            var apiKey = await _apiKeyService.GenerateApiKeyAsync(request.ClientId, request.WalletId);
            return Ok(new ApiKeyDto { Key = apiKey.Id.ToString(), Wallet = apiKey.WalletId });
        }

        /// <summary>
        /// Get all api keys for a specified client.
        /// </summary>
        /// <param name="clientId"></param>
        [HttpGet("{clientId}")]
        [SwaggerOperation("GetKeys")]
        [ProducesResponseType(typeof(ApiKeyDto[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetKeys(string clientId)
        {
            if (string.IsNullOrWhiteSpace(clientId))
                return BadRequest();

            var keys = await _apiKeyService.GetApiKeysAsync(clientId);
            return Ok(keys.Select(key => new ApiKeyDto { Key = key.Id.ToString(), Wallet = key.WalletId}));
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
    }
}
