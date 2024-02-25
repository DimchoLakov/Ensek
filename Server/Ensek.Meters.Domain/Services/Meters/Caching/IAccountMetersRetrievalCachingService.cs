using Ensek.Meters.Data.Models;

namespace Ensek.Meters.Domain.Services.Meters.Caching;

public interface IAccountMetersRetrievalCachingService
{
    Task<bool> AccountExistsAsync(
        long accountId,
        CancellationToken cancellationToken);

    Task<MeterReading> GetLatestMeterReadingByAccountId(
        long accountId,
        CancellationToken cancellationToken);

    void UpdateCache(
        long accountId,
        MeterReading meterReading);
}
