using AutoMapper;
using JetBrains.Annotations;
using Lykke.Service.HftInternalService.Core.Domain;
using Lykke.Service.HftInternalService.Models.V2;

namespace Lykke.Service.HftInternalService.Modules
{
    /// <inheritdoc />
    [UsedImplicitly]
    public class ApiKeyProfile : Profile
    {
        /// <inheritdoc />
        public ApiKeyProfile()
        {
            CreateMap<ApiKey, ApiKeyDto>()
                .ForMember(dto => dto.ApiKey, m => m.MapFrom(o => string.IsNullOrEmpty(o.Token) ? o.Id.ToString() :o.Token))
                .ForMember(dto => dto.WalletId, m => m.MapFrom(o => o.WalletId))
                .ForMember(dto => dto.Enabled, m => m.MapFrom(o => !o.ValidTill.HasValue));
        }
    }
}
