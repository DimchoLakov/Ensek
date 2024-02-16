using Ensek.Meters.Data.Models;

namespace Ensek.Meters.Data.Repositories.MeterReadings;

public interface IMeterReadingRepository
{
    Task BulkSave(
        IEnumerable<MeterReading> meterReadings,
        CancellationToken cancellationToken);
}
