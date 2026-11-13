namespace NoShoot.Api.Models;

public class Player
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string NormalizedName { get; set; } = "";
    public string TokenHash { get; set; } = "";
    public DateTime CreatedAt { get; set; }

    public List<ScoreEntry> Scores { get; set; } = [];
}