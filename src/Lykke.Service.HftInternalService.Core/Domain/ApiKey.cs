using System;

namespace Lykke.Service.HftInternalService.Core.Domain
{
    public class ApiKey : IHasId
    {
        public Guid Id { get; set; }
        public string ClientId { get; set; }
        public DateTime? ValidTill { get; set; }
    }
}
