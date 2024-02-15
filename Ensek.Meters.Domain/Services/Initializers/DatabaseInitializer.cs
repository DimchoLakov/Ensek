using Ensek.Meters.Data;
using Microsoft.EntityFrameworkCore;

namespace Ensek.Meters.Domain.Services.Initializers;

public class DatabaseInitializer : IInitializer
{
    private readonly EnsekDbContext _dbContext;

    public DatabaseInitializer(EnsekDbContext db)
    {
        _dbContext = db;
    }

    public void Initialize()
    {
        if (_dbContext.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
        {
            return;
        }

        _dbContext.Database.Migrate();
    }
}
