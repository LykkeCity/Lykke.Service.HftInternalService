using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lykke.Service.HftInternalService.Core.Services;
using Lykke.Service.HftInternalService.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.HftInternalService.Controllers
{
    [Route("api/[controller]")]
    public class KeysController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IApiKeyService _apiKeyService;

        public KeysController(IAccountService accountService, IApiKeyService apiKeyService)
        {
            _apiKeyService = apiKeyService ?? throw new ArgumentNullException(nameof(apiKeyService));
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
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
            var apiKey = await _accountService.CreateAccount(request.ClientId);
            return Ok(new ApiKeyDto { Key = apiKey.Id.ToString(), Wallet = apiKey.AccountId });
        }

        /// <summary>
        /// Get all api keys for a specified client.
        /// </summary>
        /// <param name="clientId"></param>
        [HttpGet("{clientId}")]
        [ProducesResponseType(typeof(ApiKeyDto[]), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetKeys(string clientId)
        {
            var keys = await _apiKeyService.GetApiKeysAsync(clientId);
            return Ok(keys.Select(key => new ApiKeyDto { Key = key.Id.ToString(), Wallet = key.AccountId ?? key.ClientId }));   // remove ClientId usage here
        }

        /// <summary>
        /// Delete specified api-key.
        /// </summary>
        /// <param name="key"></param>
        [HttpDelete("{key}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteKey(string key)
        {
            await _accountService.DeleteAccount(key);
            return Ok();
        }
    }
}
