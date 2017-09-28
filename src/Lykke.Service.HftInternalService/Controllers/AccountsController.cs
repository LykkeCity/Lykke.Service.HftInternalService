using System;
using System.Net;
using System.Threading.Tasks;
using Lykke.Service.HftInternalService.Core;
using Lykke.Service.HftInternalService.Core.Domain;
using Lykke.Service.HftInternalService.Core.Services;
using Lykke.Service.HftInternalService.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.HftInternalService.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    public class AccountsController : Controller
    {
        private readonly HighFrequencyTradingSettings _settings;
        private readonly IAccountService _accountService;
        private readonly IApiKeyService _apiKeyService;

        public AccountsController(HighFrequencyTradingSettings settings, IAccountService accountService, IApiKeyService apiKeyService)
        {
            _apiKeyService = apiKeyService ?? throw new ArgumentNullException(nameof(apiKeyService));
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        /// <summary>
        /// Create HFT account for specified client.
        /// </summary>
        /// <param name="request">Account creation settings.</param>
        /// <returns>Trusted account ID and API key.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Account), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
        {
            var account = await _accountService.CreateAccount(request.ClientId);
            return Ok(account);
        }

        /// <summary>
        /// Get HFT account.
        /// </summary>
        /// <param name="id">Client ID</param>
        /// <returns>Trusted account DTO.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Account), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetAccount(string id)
        {
            var account = await _accountService.GetAccount(id);
            return Ok(account);
        }

        /// <summary>
        /// Get all api keys for a specified account.
        /// </summary>
        /// <param name="accountId"></param>
        [HttpGet("{accountId}/keys")]
        [ProducesResponseType(typeof(ApiKey[]), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetKeys(string accountId)
        {
            var keys = await _apiKeyService.GetApiKeysAsync(accountId);
            return Ok(keys);
        }
        
        /// <summary>
        /// Cash-in/out. Only for testing purpose. Should be removed.
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="assetId"></param>
        /// <param name="amount"></param>
        /// <returns>Trusted account balances.</returns>
        /// <remarks>Please use service-defined access token as 'api-key'.</remarks>
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
