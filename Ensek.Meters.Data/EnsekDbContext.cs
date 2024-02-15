using Ensek.Meters.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Ensek.Meters.Data;

public class EnsekDbContext : DbContext
{
    public EnsekDbContext(DbContextOptions<EnsekDbContext> options)
       : base(options)
    {
    }

    public DbSet<Account> Accounts { get; set; }

    public DbSet<MeterReading> MeterReadings { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }
}
