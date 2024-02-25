using Ensek.Meters.Data.Models;
using Ensek.Meters.Domain.Models;
using static Ensek.Meters.Domain.Constants;

namespace Ensek.Meters.Domain.Services.Meters.Filtering;

public class MeterFilteringService : IMeterFilteringService
{
    public MeterReadingCsv[] GetValidRecords(
        IGrouping<long, MeterReadingCsv> group,
        MeterReading latestMeterReading)
    {
        var latestMeterReadingDateTime = latestMeterReading?.MeterReadingDateTime;

        // Get readings with values which have 5 digits in total
        var validRecordsQuery = group.Where(
            x => Math.Abs(x.MeterReadValue).ToString().Length == DefaultMeterReadingValueCount);

        if (latestMeterReadingDateTime != null)
        {
            // Further filter the query to get the readings newer than the latest one
            validRecordsQuery = validRecordsQuery.Where(
                x => latestMeterReadingDateTime.Value < x.MeterReadingDateTime);
        }

        // Make sure we don't get duplicates
        var validRecords = validRecordsQuery
            .DistinctBy(x => new { x.MeterReadValue, x.MeterReadingDateTime })
            .ToArray();

        return validRecords;
    }
}
