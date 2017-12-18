using System;

namespace Lykke.Service.HftInternalService.Contracts.Events
{
    public class ApiKey
    {
        public Guid Id { get; set; }
        public string WalletId { get; set; }
        public bool Enabled { get; set; }
    }
}
