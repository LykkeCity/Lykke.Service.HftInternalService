using System;
using System.Net;
using System.Threading.Tasks;
using Lykke.Service.HftInternalService.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.HftInternalService.Controllers
{
    [Route("api/[controller]")]
    public class AccountsController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
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
