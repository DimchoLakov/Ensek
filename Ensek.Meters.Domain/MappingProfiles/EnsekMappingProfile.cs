using AutoMapper;
using Ensek.Meters.Data.Models;
using Ensek.Meters.Domain.Models;

namespace Ensek.Meters.Domain.MappingProfiles;

public class EnsekMappingProfile : Profile
{
    public EnsekMappingProfile()
    {
        CreateMap<MeterReadingCsv, MeterReading>()
            .ForMember(x => x.Id, opt => opt.Ignore())
            .ForMember(x => x.Account, opt => opt.Ignore())
            .ForMember(x => x.MeterRead, opt => opt.MapFrom(src => src.MeterReadValue));

        CreateMap<AccountCsv, Account>()
            .ForMember(x => x.Id, opt => opt.MapFrom(src => src.AccountId));
    }
}
