using Ensek.Meters.Data.Models;

namespace Ensek.Meters.Data.Repositories.MeterReadings;

public class MeterReadingRepository : IMeterReadingRepository
{
    private readonly EnsekDbContext _dbContext;

    public MeterReadingRepository(EnsekDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task BulkSave(
        IEnumerable<MeterReading> meterReadings,
        CancellationToken cancellationToken)
    {
        await _dbContext
            .MeterReadings
            .AddRangeAsync(meterReadings, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
