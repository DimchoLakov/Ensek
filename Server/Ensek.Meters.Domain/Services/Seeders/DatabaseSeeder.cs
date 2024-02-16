using AutoMapper;
using Ensek.Meters.Data;
using Ensek.Meters.Data.Models;
using Ensek.Meters.Domain.Models;
using Ensek.Meters.Domain.Services.Csv;
using Microsoft.EntityFrameworkCore;

namespace Ensek.Meters.Domain.Services.Seeders;

public class DatabaseSeeder : IDataSeeder
{
    private readonly ICsvReaderService _csvReaderService;
    private readonly EnsekDbContext _dbContext;
    private readonly IMapper _mapper;

    public DatabaseSeeder(
        ICsvReaderService csvReaderService,
        EnsekDbContext dbContext,
        IMapper mapper)
    {
        _csvReaderService = csvReaderService;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task Seed()
    {
        if (_dbContext.Database.ProviderName == Constants.InMemoryProviderName)
        {
            return;
        }

        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var fullPath = Path.GetFullPath(
            "./SeedFiles/Test_Accounts.csv",
            basePath);

        var file = File.OpenRead(fullPath);
        var batches = _csvReaderService.ReadCsvFileInBatches<AccountCsv>(file, Constants.DefaultBatchSize);
        var accounts = new List<Account>();

        await foreach (var batch in batches)
        {
            foreach (var account in batch)
            {
                var mappedItem = _mapper.Map<Account>(account);
                accounts.Add(mappedItem);
            }
        }

        if (!_dbContext.Accounts.Any())
        {
            try
            {
                await _dbContext
                    .Database
                    .OpenConnectionAsync();

                await _dbContext
                    .Database
                    .ExecuteSqlRawAsync("SET IDENTITY_INSERT Accounts ON");

                await _dbContext
                    .Accounts
                    .AddRangeAsync(accounts);

                await _dbContext.SaveChangesAsync();
            }
            finally
            {
                await _dbContext
                    .Database
                    .ExecuteSqlRawAsync("SET IDENTITY_INSERT Accounts OFF");

                await _dbContext
                    .Database
                    .CloseConnectionAsync();
            }
        }
    }
}
