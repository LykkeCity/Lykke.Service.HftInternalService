using AutoMapper;
using JetBrains.Annotations;
using Lykke.Service.HftInternalService.Core.Domain;

namespace Lykke.Service.HftInternalService.Modules
{
    /// <inheritdoc />
    [UsedImplicitly]
    public class ApiKeyProfile : Profile
    {
        /// <inheritdoc />
        public ApiKeyProfile()
        {
            CreateMap<ApiKey, Models.V2.ApiKeyDto>()
                .ForMember(dto => dto.Id, m => m.MapFrom(o => o.Id))
                .ForMember(dto => dto.ApiKey, m => m.MapFrom(o => string.IsNullOrEmpty(o.Token) ? o.Id.ToString() : o.Token))
                .ForMember(dto => dto.WalletId, m => m.MapFrom(o => o.WalletId))
                .ForMember(dto => dto.ClientId, m => m.MapFrom(o => o.ClientId))
                .ForMember(dto => dto.Enabled, m => m.MapFrom(o => !o.ValidTill.HasValue))
                .ForMember(dto => dto.Apiv2Only, m => m.MapFrom(o => o.Apiv2Only));

            CreateMap<ApiKey, Models.v1.ApiKeyDto>()
                .ForMember(dto => dto.Key, m => m.MapFrom(o => string.IsNullOrEmpty(o.Token) ? o.Id.ToString() : o.Token))
                .ForMember(dto => dto.Wallet, m => m.MapFrom(o => o.WalletId))
                .ForMember(dto => dto.Apiv2Only, m => m.MapFrom(o => o.Apiv2Only));
        }
    }
}
