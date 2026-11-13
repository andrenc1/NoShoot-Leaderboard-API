using Microsoft.EntityFrameworkCore;
using NoShoot.Api.Models;
using NoShoot.Api.Validation;

namespace NoShoot.Api.Data;

public class LeaderboardDbContext : DbContext
{
    public LeaderboardDbContext(DbContextOptions<LeaderboardDbContext> options)
        : base(options)
    {
    }

    public DbSet<Player> Players => Set<Player>();
    public DbSet<ScoreEntry> Scores => Set<ScoreEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Player>(player =>
        {
            player.Property(p => p.Name).HasMaxLength(PlayerNameValidator.MaxLength);
            player.Property(p => p.NormalizedName).HasMaxLength(PlayerNameValidator.MaxLength);

            player.HasIndex(p => p.NormalizedName).IsUnique();
            player.HasIndex(p => p.TokenHash).IsUnique();
        });

        modelBuilder.Entity<ScoreEntry>()
            .HasOne(s => s.Player)
            .WithMany(p => p.Scores)
            .HasForeignKey(s => s.PlayerId);
    }
}