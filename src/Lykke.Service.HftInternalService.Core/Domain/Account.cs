using System;

namespace Lykke.Service.HftInternalService.Core.Domain
{
    public class Account : IHasId
    {
        public Guid Id { get; set; }
        public string ClientId { get; set; }
        public string ApiKey { get; set; }
        //public ApiKey[] ApiKeys { get; set; }
    }
}
