using MessagePack;

namespace Lykke.Service.HftInternalService.Services.Events
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class ApiKeyUpdatedEvent
    {
        public string ApiKey { get; set; }
        public string Token { get; set; }
        public string WalletId { get; set; }
        public bool Enabled { get; set; }
        public bool Apiv2Only { get; set; }
        public string ClientId { get; set; }
    }
}
