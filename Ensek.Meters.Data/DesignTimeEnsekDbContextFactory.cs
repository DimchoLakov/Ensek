using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Ensek.Meters.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<EnsekDbContext>
{
    public EnsekDbContext CreateDbContext(string[] args)
    {
        var pathToWebProject = Path.GetFullPath(Directory.GetCurrentDirectory() + "./../Ensek.Api/");

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(pathToWebProject)
            .AddJsonFile("appsettings.json")
            .Build();

        var builder = new DbContextOptionsBuilder<EnsekDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        builder.UseSqlServer(connectionString);

        return new EnsekDbContext(builder.Options);
    }
}
