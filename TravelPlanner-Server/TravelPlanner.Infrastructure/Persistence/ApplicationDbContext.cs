using Microsoft.EntityFrameworkCore;
using TravelPlanner.Domain.Entities;

namespace TravelPlanner.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // הגדרת הטבלאות שיהיו במסד הנתונים
    public DbSet<User> Users { get; set; }
    public DbSet<Trip> Trips { get; set; }
    public DbSet<TripPlace> TripPlaces { get; set; }
    public DbSet<SavedPlace> SavedPlaces { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)

    {
        // מוודא שתעודת זהות תהיה ייחודית
        modelBuilder.Entity<User>()
            .HasIndex(u => u.IdentityNumber)
            .IsUnique();

        base.OnModelCreating(modelBuilder);
    }
}