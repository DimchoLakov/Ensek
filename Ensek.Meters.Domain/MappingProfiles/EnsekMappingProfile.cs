using AutoMapper;
using Ensek.Meters.Data.Models;
using Ensek.Meters.Domain.Models;
using System.Globalization;

namespace Ensek.Meters.Domain.MappingProfiles;

public class EnsekMappingProfile : Profile
{
    public EnsekMappingProfile()
    {
        CreateMap<MeterReadingCsv, MeterReading>()
            .ForMember(x => x.Id, opt => opt.Ignore())
            .ForMember(x => x.Account, opt => opt.Ignore())
            .ForMember(x => x.MeterRead, opt => opt.MapFrom(src => src.MeterReadValue))
            .ForMember(x => x.MeterReadingDateTime, opt => opt.MapFrom(
                src => DateTime.ParseExact(src.MeterReadingDateTime, "dd/MM/yyyy hh:mm", CultureInfo.InvariantCulture)));

        CreateMap<AccountCsv, Account>()
            .ForMember(x => x.Id, opt => opt.MapFrom(src => src.AccountId));
    }
}
