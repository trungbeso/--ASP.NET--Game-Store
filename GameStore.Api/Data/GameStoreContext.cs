using GameStore.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data;

public class GameStoreContext(DbContextOptions<GameStoreContext> options) : DbContext(options)
{
    public DbSet<Game> Games => Set<Game>();
    public DbSet<Genre> Genres => Set<Genre>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
        modelBuilder.Entity<Genre>().HasData(
            new {Id = 1, Name ="Action-adventure"},
            new {Id = 2, Name ="Platformer"},
            new {Id = 3, Name ="Action RPG"},
            new {Id = 4, Name ="Comedy"},
            new {Id = 5, Name ="Metroidvania"}
        );
  }
}
