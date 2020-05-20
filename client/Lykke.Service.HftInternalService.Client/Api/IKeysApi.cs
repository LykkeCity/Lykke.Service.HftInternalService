using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.HftInternalService.Client.Keys;
using Refit;

namespace Lykke.Service.HftInternalService.Client.Api
{
    /// <summary>
    /// Service interface for the HFT internal service api-key functionality.
    /// </summary>
    [PublicAPI]
    public interface IKeysApi
    {
        /// <summary>
        /// Creates a new api wallet.
        /// </summary>
        /// <param name="data">the information of the new api wallet</param>
        [Post("/api/v2/keys")]
        Task<ApiKeyModel> CreateKey([Body, Required] CreateApiKeyModel data);

        /// <summary>
        /// Updates the api-key of an existing wallet.
        /// </summary>
        /// <param name="data">the information of the api wallet to update</param>
        [Post("/api/v2/keys/new")]
        Task<ApiKeyModel> UpdateKey([Body, Required] UpdateApiKeyModel data);

        /// <summary>
        /// Deletes an api wallet.
        /// </summary>
        /// <param name="key">the api key to delete</param>
        [Delete("/api/v2/keys/{key}")]
        Task DeleteKey([Required] string key);

        /// <summary>
        /// Gets the api wallet information for the given api key.
        /// </summary>
        /// <param name="key">the api key to get</param>
        [Get("/api/v2/keys/{key}")]
        Task<ApiKeyModel> GetKey([Required] string key);

        /// <summary>
        /// Gets all api wallets of a client.
        /// </summary>
        /// <param name="clientId">the client to query for api wallets</param>
        [Get("/api/v2/keys")]
        Task<IReadOnlyCollection<ApiKeyModel>> GetKeys([Required] string clientId);

        /// <summary>
        /// Gets all api key ids
        /// </summary>
        [Get("/api/v2/keys/ids")]
        Task<IReadOnlyCollection<string>> GetAllKeyIds();
    }
}
