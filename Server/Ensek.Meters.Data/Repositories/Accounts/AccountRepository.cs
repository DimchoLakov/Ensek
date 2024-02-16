using Ensek.Meters.Data.OutputModels;
using Microsoft.EntityFrameworkCore;

namespace Ensek.Meters.Data.Repositories.Accounts;

public class AccountRepository : IAccountRepository
{
    private readonly EnsekDbContext _dbContext;

    public AccountRepository(EnsekDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ExistsAsync(
        long id,
        CancellationToken cancellationToken)
    {
        return await _dbContext
            .Accounts
            .AnyAsync(x => x.Id == id);
    }

    public async Task<AccountMeterReadingModel> GetAccountByIdWithLatestReadingAsync(
        long id,
        CancellationToken cancellationToken)
    {
        var accountWithLatestReading = await _dbContext
            .Accounts
            .Include(x => x.MeterReadings)
            .Where(x => x.Id == id)
            .Select(x => new AccountMeterReadingModel
            {
                AccountId = x.Id,
                LatestMeterReading = x.MeterReadings
                .OrderByDescending(m => m.MeterReadingDateTime)
                .FirstOrDefault()
            })
            .FirstOrDefaultAsync();

        return accountWithLatestReading;
    }
}
