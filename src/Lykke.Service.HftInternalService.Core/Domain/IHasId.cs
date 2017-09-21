using System;

namespace Lykke.Service.HftInternalService.Core.Domain
{
    public interface IHasId
    {
        Guid Id { get; set; }
    }
}
