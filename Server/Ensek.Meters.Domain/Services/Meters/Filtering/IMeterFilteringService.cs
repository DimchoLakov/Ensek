using Ensek.Meters.Data.Models;
using Ensek.Meters.Domain.Models;

namespace Ensek.Meters.Domain.Services.Meters.Filtering;

public interface IMeterFilteringService
{
    MeterReadingCsv[] GetValidRecords(
        IGrouping<long, MeterReadingCsv> group,
        MeterReading latestMeterReading);
}
