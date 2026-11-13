using Microsoft.EntityFrameworkCore;
using NoShoot.Api.Contracts;
using NoShoot.Api.Data;
using NoShoot.Api.Models;
using NoShoot.Api.Scoring;
using NoShoot.Api.Security;
using NoShoot.Api.Validation;

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

// ---------- Jogadores ----------

app.MapPost("/api/players", async (RegisterPlayerRequest request, LeaderboardDbContext db) =>
{
    var errors = PlayerNameValidator.Validate(request.Name);
    if (errors.Count > 0)
        return Results.ValidationProblem(errors);

    string name = request.Name.Trim();
    string normalizedName = PlayerNameValidator.Normalize(name);

    bool nameTaken = await db.Players.AnyAsync(p => p.NormalizedName == normalizedName);
    if (nameTaken)
        return Results.Conflict(new { error = "This name is already taken." });

    string token = PlayerToken.Generate();

    var player = new Player
    {
        Name = name,
        NormalizedName = normalizedName,
        TokenHash = PlayerToken.Hash(token),
        CreatedAt = DateTime.UtcNow
    };

    db.Players.Add(player);

    try
    {
        await db.SaveChangesAsync();
    }
    catch (DbUpdateException)
    {
        return Results.Conflict(new { error = "This name is already taken." });
    }

    return Results.Created($"/api/players/{player.Id}",
        new RegisterPlayerResponse(player.Id, player.Name, token));
});

// ---------- Pontuações ----------

app.MapPost("/api/scores", async (SubmitScoreRequest request, HttpRequest http, LeaderboardDbContext db) =>
{
    string? token = BearerToken.Read(http.Headers.Authorization.ToString());
    if (token is null)
        return Results.Unauthorized();

    string tokenHash = PlayerToken.Hash(token);
    var player = await db.Players.FirstOrDefaultAsync(p => p.TokenHash == tokenHash);
    if (player is null)
        return Results.Unauthorized();

    var errors = ScoreValidator.Validate(request);
    if (errors.Count > 0)
        return Results.ValidationProblem(errors);

    var entry = new ScoreEntry
    {
        PlayerId = player.Id,
        Kills = request.Kills,
        SurvivedSeconds = request.SurvivedSeconds,
        Score = ScoreCalculator.Calculate(request.Kills, request.SurvivedSeconds),
        CreatedAt = DateTime.UtcNow
    };

    db.Scores.Add(entry);
    await db.SaveChangesAsync();

    var response = new ScoreResponse(
        entry.Id, player.Name, entry.Kills, entry.SurvivedSeconds, entry.Score, entry.CreatedAt);

    return Results.Created($"/api/scores/{entry.Id}", response);
});

app.MapGet("/api/scores/{id:int}", async (int id, LeaderboardDbContext db) =>
{
    var score = await db.Scores
        .Where(s => s.Id == id)
        .Select(s => new ScoreResponse(
            s.Id, s.Player.Name, s.Kills, s.SurvivedSeconds, s.Score, s.CreatedAt))
        .FirstOrDefaultAsync();

    return score is null ? Results.NotFound() : Results.Ok(score);
});

app.MapGet("/api/scores/top", async (LeaderboardDbContext db, int limit = 10) =>
{
    limit = Math.Clamp(limit, 1, 100);

    return await db.Players
        .Where(p => p.Scores.Any())
        .OrderByDescending(p => p.Scores.Max(s => s.Score))
        .Take(limit)
        .Select(p => new LeaderboardEntry(p.Name, p.Scores.Max(s => s.Score)))
        .ToListAsync();
});

app.Run();