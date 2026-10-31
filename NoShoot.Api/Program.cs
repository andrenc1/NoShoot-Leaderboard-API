using Microsoft.EntityFrameworkCore;
using NoShoot.Api.Data;
using NoShoot.Api.Models;
using NoShoot.Api.Validation;
using NoShoot.Api.Scoring;

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

app.MapGet("/", () => "NoShoot Leaderboard API");

app.MapPost("/api/scores", async (SubmitScoreRequest request, LeaderboardDbContext db) =>
{
    var errors = ScoreValidator.Validate(request);
    if (errors.Count > 0)
        return Results.ValidationProblem(errors);

    var entry = new ScoreEntry
    {
        PlayerName = request.PlayerName.Trim(),
        Kills = request.Kills,
        SurvivedSeconds = request.SurvivedSeconds,
        Score = ScoreCalculator.Calculate(request.Kills, request.SurvivedSeconds),
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
{
    limit = Math.Clamp(limit, 1, 100);

    return await db.Scores
        .OrderByDescending(s => s.Score)
        .Take(limit)
        .ToListAsync();
});

app.Run();
