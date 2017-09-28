using System;
using System.Net;
using System.Threading.Tasks;
using Lykke.Service.HftInternalService.Core;
using Lykke.Service.HftInternalService.Core.Domain;
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
        private readonly IAccountService _accountService;

        public AccountsController(HighFrequencyTradingSettings settings, IAccountService accountService)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        /// <summary>
        /// Create HFT account for specified client.
        /// </summary>
        /// <param name="request">Account creation settings.</param>
        /// <returns>Trusted account ID and API key.</returns>
        /// <remarks>Please use service-defined access token as 'api-key'.</remarks>
        [HttpPost]
        [ProducesResponseType(typeof(Account), (int)HttpStatusCode.OK)]
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

            var account = await _accountService.CreateAccount(request.ClientId);
            return Ok(account);
        }

        /// <summary>
        /// Get HFT account.
        /// </summary>
        /// <param name="id">Client ID</param>
        /// <returns>Trusted account DTO.</returns>
        /// <remarks>Please use service-defined access token as 'api-key'.</remarks>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Account), (int)HttpStatusCode.OK)]
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

            var account = await _accountService.GetAccount(id);
            return Ok(account);
        }

        /// <summary>
        /// Delete HFT account.
        /// </summary>
        /// <param name="id">Client ID</param>
        /// <remarks>Please use service-defined access token as 'api-key'.</remarks>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> DeleteAccount(string id)
        {
            // todo: check admin api key
            // todo: use request headers for auth
            //var userId = User.GetUserId();
            //if (userId != _settings.ApiKey)
            //{
            //    return Forbid();
            //}

            var account = await _accountService.GetAccount(id);
            return Ok(new AccountDto { ApiKey = account.ApiKey, Id = account.Id });
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
