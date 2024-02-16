using Ensek.Meters.Data.OutputModels;

namespace Ensek.Meters.Data.Repositories.Accounts;

public interface IAccountRepository
{
    Task<bool> ExistsAsync(
        long id,
        CancellationToken cancellationToken);

    Task<AccountMeterReadingModel> GetAccountByIdWithLatestReadingAsync(
        long id,
        CancellationToken cancellationToken);
}
