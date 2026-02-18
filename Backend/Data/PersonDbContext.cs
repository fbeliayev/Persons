using Microsoft.EntityFrameworkCore;
using PersonApi.Models;

namespace PersonApi.Data;

public class PersonDbContext : DbContext
{
    private static int _nextPersonId = 1;

    public PersonDbContext(DbContextOptions<PersonDbContext> options) : base(options)
    {
    }

    public DbSet<Person> Persons { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<PersonCity> PersonCities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<PersonCity>(entity =>
        {
            entity.HasKey(pc => new { pc.PersonId, pc.CityId });

            entity.HasOne(pc => pc.Person)
                .WithMany(p => p.PersonCities)
                .HasForeignKey(pc => pc.PersonId);

            entity.HasOne(pc => pc.City)
                .WithMany()
                .HasForeignKey(pc => pc.CityId);

            // Add indexes for performance
            entity.HasIndex(pc => pc.PersonId);
            entity.HasIndex(pc => pc.CityId);
            entity.HasIndex(pc => pc.IsVisited);
        });

        // Seed 10 cities
        modelBuilder.Entity<City>().HasData(
            new City { Id = 1, Name = "Paris", Country = "France" },
            new City { Id = 2, Name = "Tokyo", Country = "Japan" },
            new City { Id = 3, Name = "New York", Country = "USA" },
            new City { Id = 4, Name = "Barcelona", Country = "Spain" },
            new City { Id = 5, Name = "Dubai", Country = "UAE" },
            new City { Id = 6, Name = "London", Country = "UK" },
            new City { Id = 7, Name = "Rome", Country = "Italy" },
            new City { Id = 8, Name = "Sydney", Country = "Australia" },
            new City { Id = 9, Name = "Istanbul", Country = "Turkey" },
            new City { Id = 10, Name = "Amsterdam", Country = "Netherlands" }
        );
    }

    public override int SaveChanges()
    {
        AssignIds();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AssignIds();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void AssignIds()
    {
        var personEntries = ChangeTracker.Entries<Person>()
            .Where(e => e.State == EntityState.Added && e.Entity.Id == 0);

        foreach (var entry in personEntries)
        {
            entry.Entity.Id = _nextPersonId++;
        }
    }
}
