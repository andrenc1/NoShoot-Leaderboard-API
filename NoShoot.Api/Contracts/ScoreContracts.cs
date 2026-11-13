namespace NoShoot.Api.Contracts;

public record SubmitScoreRequest(int Kills, int SurvivedSeconds);

public record ScoreResponse(int Id, string PlayerName, int Kills, int SurvivedSeconds, int Score, DateTime CreatedAt);

public record LeaderboardEntry(string PlayerName, int BestScore);