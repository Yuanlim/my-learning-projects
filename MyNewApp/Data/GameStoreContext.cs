using Microsoft.EntityFrameworkCore;
using MyNewApp.Entities;

namespace MyNewApp.Data;

// DbContextOptions<GameStoreContext> options: all settings
public class GameStoreContext(DbContextOptions<GameStoreContext> options) : DbContext(options)
{
    public DbSet<Game> Games => Set<Game>(); // A table: Games table
    public DbSet<Genre> Genres => Set<Genre>();
    protected override void OnModelCreating(ModelBuilder modelBuilder) // OnModelCreating is called when migrating
    {
        // Instead of data notation, I can define each field limitation in OnModelCreating
        // modelBuilder.Entity<Game>(game =>
        // {
        //     game.HasKey(entities => entities.Id); // Def: Game Id is primary key
        //     game.Property(entities => entities.Name).HasMaxLength(100).IsRequired(); // Def: game name must be lower than 100 character and is required
        //     game.HasOne(entities => entities.Genre);
        //     game.Property(entities => entities.Genre).HasMaxLength(20);
        // });

        // Seed data: HasData registers initial rows; migrations will generate
        modelBuilder.Entity<Genre>().HasData(
            new { Id = 1, Name = "Fighting" },
            new { Id = 2, Name = "Roleplaying" },
            new { Id = 3, Name = "Sports" },
            new { Id = 4, Name = "Racing" },
            new { Id = 5, Name = "Kids and Family" }
        );
    }
}
