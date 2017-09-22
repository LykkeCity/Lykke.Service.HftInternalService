using System;
using System.Net;
using System.Threading.Tasks;
using Lykke.Service.HftInternalService.Core;
using Lykke.Service.HftInternalService.Core.Services;
using Lykke.Service.HftInternalService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.HftInternalService.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    public class AccountsController : Controller
    {
        private readonly HighFrequencyTradingSettings _settings;
        private readonly ITrustedAccountService _trustedAccountService;

        public AccountsController(HighFrequencyTradingSettings settings, ITrustedAccountService trustedAccountService)
        {
            _trustedAccountService = trustedAccountService ?? throw new ArgumentNullException(nameof(trustedAccountService));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        /// <summary>
        /// Create trusted account for specified client.
        /// </summary>
        /// <param name="request">Account creation settings.</param>
        /// <returns>Trusted account ID and API key.</returns>
        /// <remarks>Please use service-defined access token as 'api-key'.</remarks>
        [HttpPost]
        [ProducesResponseType(typeof(AccountDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
        {
            // todo: use request headers for auth
            //var userId = User.GetUserId();
            var userId = request.AdminApiKey;
            if (userId != _settings.ApiKey)
            {
                //return Forbid();
                return Unauthorized();
            }

            var account = await _trustedAccountService.CreateAccount(request.ClientId);
            return Ok(new AccountDto { ApiKey = account.ApiKey, Id = account.Id });
        }

        /// <summary>
        /// Get trusted account.
        /// </summary>
        /// <param name="id">Client ID</param>
        /// <returns>Trusted account DTO.</returns>
        /// <remarks>Please use service-defined access token as 'api-key'.</remarks>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AccountDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetAccount(string id)
        {
            // todo: check admin api key
            // todo: use request headers for auth
            //var userId = User.GetUserId();
            //if (userId != _settings.ApiKey)
            //{
            //    return Forbid();
            //}

            var account = await _trustedAccountService.GetAccount(id);
            return Ok(new AccountDto { ApiKey = account.ApiKey, Id = account.Id });
        }

        /// <summary>
        /// Get trusted account balances.
        /// </summary>
        /// <param name="id">Client ID</param>
        /// <returns>Trusted account balances.</returns>
        /// <remarks>Please use service-defined access token as 'api-key'.</remarks>
        [HttpGet("{id}/balances")]
        [ProducesResponseType(typeof(AccountDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetAccountBalances(string id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// CashInOut.
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
            var result = await _trustedAccountService.CashInOut(accountId, assetId, amount);
            return Ok(result);
        }
    }
}
