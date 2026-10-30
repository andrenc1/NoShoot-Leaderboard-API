using Microsoft.EntityFrameworkCore;
using NoShoot.Api.Models;

namespace NoShoot.Api.Data;

public class LeaderboardDbContext : DbContext
{
    public LeaderboardDbContext(DbContextOptions<LeaderboardDbContext> options)
        : base(options)
    {
    }

    public DbSet<ScoreEntry> Scores => Set<ScoreEntry>();
}