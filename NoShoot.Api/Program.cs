using Microsoft.EntityFrameworkCore;
using NoShoot.Api.Data;
using NoShoot.Api.Models;

var builder = WebApplication.CreateBuilder(args);

var dbPath = Path.Combine(builder.Environment.ContentRootPath, "leaderboard.db");
builder.Services.AddDbContext<LeaderboardDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LeaderboardDbContext>();
    db.Database.Migrate();
}

const int PointsPerKill = 10;
const int PointsPerSecond = 1;

app.MapGet("/", () => "NoShoot Leaderboard API");

app.MapPost("/api/scores", async (SubmitScoreRequest request, LeaderboardDbContext db) =>
{
    var entry = new ScoreEntry
    {
        PlayerName = request.PlayerName,
        Kills = request.Kills,
        SurvivedSeconds = request.SurvivedSeconds,
        Score = request.Kills * PointsPerKill + request.SurvivedSeconds * PointsPerSecond,
        CreatedAt = DateTime.UtcNow
    };

    db.Scores.Add(entry);
    await db.SaveChangesAsync();

    return Results.Created($"/api/scores/{entry.Id}", entry);
});

app.MapGet("/api/scores/{id:int}", async (int id, LeaderboardDbContext db) =>
    await db.Scores.FindAsync(id) is ScoreEntry entry
        ? Results.Ok(entry)
        : Results.NotFound());

app.MapGet("/api/scores/top", async (LeaderboardDbContext db, int limit = 10) =>
    await db.Scores
        .OrderByDescending(s => s.Score)
        .Take(limit)
        .ToListAsync());

app.Run();

record SubmitScoreRequest(string PlayerName, int Kills, int SurvivedSeconds);