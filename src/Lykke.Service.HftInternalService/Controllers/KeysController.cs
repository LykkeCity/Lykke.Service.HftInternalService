using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lykke.Service.HftInternalService.Core.Services;
using Lykke.Service.HftInternalService.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.SwaggerGen.Annotations;

namespace Lykke.Service.HftInternalService.Controllers
{
    [Route("api/[controller]")]
    public class KeysController : Controller
    {
        private readonly IWalletService _walletService;
        private readonly IApiKeyService _apiKeyService;

        public KeysController(IWalletService walletService, IApiKeyService apiKeyService)
        {
            _apiKeyService = apiKeyService ?? throw new ArgumentNullException(nameof(apiKeyService));
            _walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
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

            var apiKey = await _walletService.CreateWallet(request.ClientId, request.Name);
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
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> RegenerateKey([FromBody] RegenerateKeyRequest request)
        {
            if (request == null)
                return BadRequest();

            var existingKey = (await _apiKeyService.GetApiKeysAsync(request.ClientId)).FirstOrDefault(x => x.WalletId == request.WalletId);
            if (existingKey == null)
                return NotFound();

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
            return Ok(keys.Select(key => new ApiKeyDto { Key = key.Id.ToString(), Wallet = key.WalletId ?? key.ClientId }));   // remove ClientId usage here after DB migration
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
