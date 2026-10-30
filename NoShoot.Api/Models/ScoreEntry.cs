namespace NoShoot.Api.Models;

public class ScoreEntry
{
    public int Id { get; set; }
    public string PlayerName { get; set; } = "";
    public int Kills { get; set; }
    public int SurvivedSeconds { get; set; }
    public int Score { get; set; }
    public DateTime CreatedAt { get; set; }
}