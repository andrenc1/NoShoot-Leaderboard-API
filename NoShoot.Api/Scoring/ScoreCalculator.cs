namespace NoShoot.Api.Scoring;

public static class ScoreCalculator
{
    public const int PointsPerKill = 10;
    public const int PointsPerSecond = 1;

    public static int Calculate(int kills, int survivedSeconds) =>
        kills * PointsPerKill + survivedSeconds * PointsPerSecond;
}