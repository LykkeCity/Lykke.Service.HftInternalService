// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Lykke.Service.HftInternalService.Client.AutorestClient.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class LykkeServiceHftInternalServiceModelsv1CreateApiKeyRequest
    {
        /// <summary>
        /// Initializes a new instance of the
        /// LykkeServiceHftInternalServiceModelsv1CreateApiKeyRequest class.
        /// </summary>
        public LykkeServiceHftInternalServiceModelsv1CreateApiKeyRequest()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// LykkeServiceHftInternalServiceModelsv1CreateApiKeyRequest class.
        /// </summary>
        public LykkeServiceHftInternalServiceModelsv1CreateApiKeyRequest(string clientId = default(string), string name = default(string), string description = default(string))
        {
            ClientId = clientId;
            Name = name;
            Description = description;
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
        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "Description")]
        public string Description { get; set; }

    }
}
