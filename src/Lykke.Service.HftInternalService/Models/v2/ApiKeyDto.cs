namespace Lykke.Service.HftInternalService.Models.V2
{
    /// <summary>
    /// Api key model v2
    /// </summary>
    public class ApiKeyDto
    {
        /// <summary>The API key</summary>
        public string ApiKey { get; set; }

        /// <summary>The wallet id</summary>
        public string WalletId { get; set; }

        /// <summary>The client id</summary>
        public string ClientId { get; set; }

        /// <summary>Is the api key enabled</summary>
        public bool Enabled { get; set; }

        /// <summary>Is the api key for api v2 only</summary>
        public bool Apiv2Only { get; set; }
    }
}
