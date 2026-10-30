var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

const int PointsPerKill = 10;
const int PointsPerSecond = 1;

var scores = new List<ScoreEntry>();

app.MapGet("/", () => "NoShoot Leaderboard API");

app.MapPost("/api/scores", (SubmitScoreRequest request) =>
{
    int score = request.Kills * PointsPerKill + request.SurvivedSeconds * PointsPerSecond;

    var entry = new ScoreEntry(
        Id: scores.Count + 1,
        PlayerName: request.PlayerName,
        Kills: request.Kills,
        SurvivedSeconds: request.SurvivedSeconds,
        Score: score,
        CreatedAt: DateTime.UtcNow);

    scores.Add(entry);

    return Results.Created($"/api/scores/{entry.Id}", entry);
});

app.MapGet("/api/scores/top", (int limit = 10) =>
    scores.OrderByDescending(s => s.Score).Take(limit));

app.Run();

record SubmitScoreRequest(string PlayerName, int Kills, int SurvivedSeconds);

record ScoreEntry(int Id, string PlayerName, int Kills, int SurvivedSeconds, int Score, DateTime CreatedAt);