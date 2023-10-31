using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace App.DAL.EF;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        //optionsBuilder.UseSqlServer("Server=host.docker.internal,1433;Database=CosmosOdysseyDb;User=sa;Password=Hobujaama10;Encrypt=true;TrustServerCertificate=True;");
        optionsBuilder.UseSqlServer(
            "Server=cosmosodyssey.database.windows.net;Database=CosmosOdysseyDb;User=CosmosOdyssey;Password=SolarSystemTravel2023$;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False");
        return new AppDbContext(optionsBuilder.Options);
    }
}