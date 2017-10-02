﻿using System;
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
        [SwaggerOperation("CreateKey")]
        [ProducesResponseType(typeof(ApiKeyDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateKey([FromBody] CreateApiKeyRequest request)
        {
            if (request == null)
                return BadRequest();

            var apiKey = await _accountService.CreateAccount(request.ClientId, request.Name);
            return Ok(new ApiKeyDto { Key = apiKey.Id.ToString(), Wallet = apiKey.AccountId });
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
            return Ok(keys.Select(key => new ApiKeyDto { Key = key.Id.ToString(), Wallet = key.AccountId ?? key.ClientId }));   // remove ClientId usage here
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

            await _accountService.DeleteAccount(apiKey);
            return Ok();
        }
    }
}
