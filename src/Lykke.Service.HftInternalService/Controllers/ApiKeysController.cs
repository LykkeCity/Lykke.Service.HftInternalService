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
    public class ApiKeysController : Controller
    {
        private readonly HighFrequencyTradingSettings _settings;
        private readonly IApiKeyService _apiKeyService;

        public ApiKeysController(HighFrequencyTradingSettings settings, IAccountService accountService, IApiKeyService apiKeyService)
        {
            _apiKeyService = apiKeyService ?? throw new ArgumentNullException(nameof(apiKeyService));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        /// <summary>
        /// Generate api-key for a specified account.
        /// </summary>
        /// <param name="request">Key creation settings.</param>
        /// <returns>Account ID and API key.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiKey), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GenerateKey([FromBody] CreateApiKeyRequest request)
        {
            var clientId = request.ClientId;
            var apiKey = await _apiKeyService.GenerateApiKeyAsync(clientId);
            return Ok(apiKey);
        }

    }
}
