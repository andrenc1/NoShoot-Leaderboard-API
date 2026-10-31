namespace NoShoot.Api.Models;

public record SubmitScoreRequest(string PlayerName, int Kills, int SurvivedSeconds);