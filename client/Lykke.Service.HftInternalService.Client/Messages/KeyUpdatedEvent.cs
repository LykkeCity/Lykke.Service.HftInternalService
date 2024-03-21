namespace Lykke.Service.HftInternalService.Client.Messages
{
    public class KeyUpdatedEvent
    {
        public string Id { get; set; }
        public bool IsDeleted { get; set; }
        public bool Apiv2Only { get; set; }
        public string ClientId { get; set; }
        public string WalletId { get; set; }
    }
}
