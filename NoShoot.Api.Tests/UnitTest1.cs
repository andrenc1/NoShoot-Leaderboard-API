using NoShoot.Api.Scoring;

namespace NoShoot.Api.Tests;

public class ScoreCalculatorTests
{
    [Fact]
    public void Calculate_UsesKillsAndSeconds()
    {
        int score = ScoreCalculator.Calculate(kills: 62, survivedSeconds: 50);

        Assert.Equal(670, score);
    }

    [Fact]
    public void Calculate_EmptyRun_IsZero()
    {
        Assert.Equal(0, ScoreCalculator.Calculate(0, 0));
    }
}