using Ensek.Meters.Data.Models;
using Ensek.Meters.Data.Repositories.Accounts;
using System.Collections.Concurrent;

namespace Ensek.Meters.Domain.Services.Meters.Caching;

public class AccountMetersRetrievalCachingService : IAccountMetersRetrievalCachingService
{
    private readonly ConcurrentDictionary<long, MeterReading> _accountMeterReadingCache;
    private readonly IAccountRepository _accountRepository;

    public AccountMetersRetrievalCachingService(IAccountRepository accountRepository)
    {
        _accountMeterReadingCache = new ConcurrentDictionary<long, MeterReading>();
        _accountRepository = accountRepository;
    }

    public async Task<bool> AccountExistsAsync(
        long accountId,
        CancellationToken cancellationToken)
    {
        if (_accountMeterReadingCache.ContainsKey(accountId))
        {
            return true;
        }

        return await _accountRepository.ExistsAsync(
            accountId,
            cancellationToken);
    }

    public async Task<MeterReading> GetLatestMeterReadingByAccountId(
        long accountId,
        CancellationToken cancellationToken)
    {
        if (_accountMeterReadingCache.TryGetValue(accountId, out var result))
        {
            return result;
        }

        var latest = await _accountRepository.GetAccountByIdWithLatestReadingAsync(
            accountId,
            cancellationToken);

        _accountMeterReadingCache[accountId] = latest.LatestMeterReading;

        return latest.LatestMeterReading;
    }

    public void UpdateCache(
        long accountId,
        MeterReading meterReading)
    {
        if (_accountMeterReadingCache.ContainsKey(accountId))
        {
            _accountMeterReadingCache[accountId] = meterReading;
        }
    }
}
