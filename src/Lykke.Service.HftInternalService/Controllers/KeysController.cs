using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lykke.Service.HftInternalService.Core;
using Lykke.Service.HftInternalService.Core.Services;
using Lykke.Service.HftInternalService.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.HftInternalService.Controllers
{
    [Route("api/[controller]")]
    public class KeysController : Controller
    {
        private readonly HighFrequencyTradingSettings _settings;
        private readonly IAccountService _accountService;
        private readonly IApiKeyService _apiKeyService;

        public KeysController(HighFrequencyTradingSettings settings, IAccountService accountService, IApiKeyService apiKeyService)
        {
            _apiKeyService = apiKeyService ?? throw new ArgumentNullException(nameof(apiKeyService));
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        /// <summary>
        /// Generate api-key for a specified account.
        /// </summary>
        /// <param name="request">Key creation settings.</param>
        /// <returns>Account ID and API key.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiKeyDto), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
        {
            //var apiKey = await _apiKeyService.GenerateApiKeyAsync(accountId, request.Name);
            //return Ok(apiKey);
            var account = await _accountService.CreateAccount(request.ClientId);
            return Ok(new ApiKeyDto { Key = account.ApiKey.Id.ToString(), Wallet = account.ApiKey.AccountId });
        }

        /// <summary>
        /// Get all api keys for a specified client.
        /// </summary>
        /// <param name="clientId"></param>
        [HttpGet("{clientId}")]
        [ProducesResponseType(typeof(ApiKeyDto[]), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetKeys(string clientId)
        {
            // todo: implement
            var keys = await _apiKeyService.GetApiKeysAsync(clientId);
            return Ok(keys.Select(key => new ApiKeyDto { Key = key.Id.ToString(), Wallet = key.AccountId }));
        }

        /// <summary>
        /// Delete specified api-key.
        /// </summary>
        /// <param name="key"></param>
        [HttpDelete("{key}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteKey(string key)
        {
            // todo: implement using ClientAccount
            return Ok();
        }

        /// <summary>
        /// Cash-in/out. Only for testing purpose. Should be removed.
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="assetId"></param>
        /// <param name="amount"></param>
        /// <returns>Trusted account balances.</returns>
        [HttpPost("{accountId}/CashInOut")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> CashInOut(string accountId, [FromQuery] string assetId, [FromQuery] double amount)
        {
            var result = await _accountService.CashInOut(accountId, assetId, amount);
            return Ok(result);
        }
    }
}
