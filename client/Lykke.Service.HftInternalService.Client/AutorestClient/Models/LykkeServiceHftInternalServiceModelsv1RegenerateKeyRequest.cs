// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.Service.HftInternalService.Client.AutorestClient.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class LykkeServiceHftInternalServiceModelsv1RegenerateKeyRequest
    {
        /// <summary>
        /// Initializes a new instance of the
        /// LykkeServiceHftInternalServiceModelsv1RegenerateKeyRequest class.
        /// </summary>
        public LykkeServiceHftInternalServiceModelsv1RegenerateKeyRequest()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// LykkeServiceHftInternalServiceModelsv1RegenerateKeyRequest class.
        /// </summary>
        public LykkeServiceHftInternalServiceModelsv1RegenerateKeyRequest(string clientId = default(string), string walletId = default(string))
        {
            ClientId = clientId;
            WalletId = walletId;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "ClientId")]
        public string ClientId { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "WalletId")]
        public string WalletId { get; set; }

    }
}