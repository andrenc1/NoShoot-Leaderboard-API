namespace NoShoot.Api.Models;

public class ScoreEntry
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public Player Player { get; set; } = null!;
    public int Kills { get; set; }
    public int SurvivedSeconds { get; set; }
    public int Score { get; set; }
    public DateTime CreatedAt { get; set; }
}