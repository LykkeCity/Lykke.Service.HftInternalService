namespace Lykke.Service.HftInternalService.Models.v1
{
    /// <summary>
    /// Api key model v1
    /// </summary>
    public class ApiKeyDto
    {
        /// <summary>The API key</summary>
        public string Key { get; set; }

        /// <summary>The wallet id</summary>
        public string Wallet { get; set; }

        /// <summary>Is the api key for api v2 only</summary>
        public bool Apiv2Only { get; set; }
    }
}
