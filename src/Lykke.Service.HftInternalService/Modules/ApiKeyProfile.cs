using AutoMapper;
using JetBrains.Annotations;
using Lykke.Service.HftInternalService.Core.Domain;
using Lykke.Service.HftInternalService.Models.V2;

namespace Lykke.Service.HftInternalService.Modules
{
    [UsedImplicitly]
    public class ApiKeyProfile : Profile
    {
        public ApiKeyProfile()
        {
            CreateMap<ApiKey, ApiKeyDto>()
                .ForMember(dto => dto.ApiKey, m => m.MapFrom(o => o.Id.ToString()))
                .ForMember(dto => dto.WalletId, m => m.MapFrom(o => o.WalletId))
                .ForMember(dto => dto.Enabled, m => m.MapFrom(o => !o.ValidTill.HasValue));
        }
    }
}
