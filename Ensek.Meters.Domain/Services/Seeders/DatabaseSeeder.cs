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

    public void Seed()
    {
        if (_dbContext.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
        {
            return;
        }

        var fullPath = Path.GetFullPath(
            "../Ensek.Meters.Domain/SeedFiles/Test_Accounts.csv",
            Directory.GetCurrentDirectory());

        var file = File.OpenRead(fullPath);
        var records = _csvReaderService.Read<AccountCsv>(file);

        var accounts = _mapper.Map<IEnumerable<Account>>(records);

        if (!_dbContext.Accounts.Any())
        {
            try
            {
                _dbContext
                    .Database
                    .OpenConnection();

                _dbContext
                    .Database
                    .ExecuteSqlRaw("SET IDENTITY_INSERT Accounts ON");

                _dbContext
                    .Accounts
                    .AddRange(accounts);

                _dbContext.SaveChanges();
            }
            finally
            {
                _dbContext
                    .Database
                    .ExecuteSqlRaw("SET IDENTITY_INSERT Accounts OFF");

                _dbContext
                    .Database
                    .CloseConnection();
            }
        }
    }
}
