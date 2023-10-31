using App.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace App.DAL.EF;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
        this.ChangeTracker.LazyLoadingEnabled = false;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Provider>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);
        
        modelBuilder.Entity<Provider>()
            .Property(s => s.TravelTime)
            .HasConversion(new TimeSpanToStringConverter()); 
        // TimeSpanToTicksConverter overflows for some multi-leg flights
        // Ideally we would create a TimeSpanToMinutesConverter and/or use Int128
  
        var reservations = modelBuilder.Entity<Reservation>();
        reservations.Property(s => s.TotalFlightTime)
            .HasConversion(new TimeSpanToStringConverter());
        // TimeSpanToTicksConverter overflows for some multi-leg flights
        // Ideally we would create a TimeSpanToMinutesConverter and/or use Int128

        reservations.HasMany(r => r.Routes)
            .WithOne(r => r.Reservation)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Provider>()
            .HasMany(p => p.Flights)
            .WithOne(f => f.Provider)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Reservation>()
            .Property(r => r.TotalPrice)
            .HasPrecision(18, 2);
    }

    public DbSet<Provider> Providers { get; set; } = default!;
    public DbSet<Company> Companies { get; set; } = default!;
    public DbSet<RouteInfo> RouteInfos { get; set; } = default!;
    public DbSet<PriceList> PriceLists  { get; set; } = default!;
    
    public DbSet<Reservation> Reservations { get; set; } = default!;
}
