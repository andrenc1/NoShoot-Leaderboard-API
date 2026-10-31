using NoShoot.Api.Models;
using NoShoot.Api.Validation;

namespace NoShoot.Api.Tests;

public class ScoreValidatorTests
{
    [Fact]
    public void ValidRequest_HasNoErrors()
    {
        var request = new SubmitScoreRequest("Andre", 62, 50);

        var errors = ScoreValidator.Validate(request);

        Assert.Empty(errors);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Abc")]
    [InlineData("Abcdefghijklmnopqrstu")]
    [InlineData("<script>")]
    [InlineData("André")]
    [InlineData("Ana Silva")]
    [InlineData("Ana-Maria")]
    public void InvalidName_IsRejected(string name)
    {
        var errors = ScoreValidator.Validate(new SubmitScoreRequest(name, 10, 60));

        Assert.True(errors.ContainsKey("playerName"));
    }

    [Theory]
    [InlineData("Abcd")]
    [InlineData("Abcdefghijklmnopqrst")]
    [InlineData("Andre")]
    [InlineData("Player_1")]
    [InlineData("ANDRE_2026")]
    [InlineData(" Andre ")]
    public void ValidName_IsAccepted(string name)
    {
        var errors = ScoreValidator.Validate(new SubmitScoreRequest(name, 10, 60));

        Assert.False(errors.ContainsKey("playerName"));
    }

    [Fact]
    public void NegativeKills_IsRejected()
    {
        var errors = ScoreValidator.Validate(new SubmitScoreRequest("Andre", -1, 60));

        Assert.True(errors.ContainsKey("kills"));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(601)]
    public void SecondsOutOfRange_IsRejected(int seconds)
    {
        var errors = ScoreValidator.Validate(new SubmitScoreRequest("Andre", 0, seconds));

        Assert.True(errors.ContainsKey("survivedSeconds"));
    }

    [Fact]
    public void FullMatch_IsAccepted()
    {
        var errors = ScoreValidator.Validate(new SubmitScoreRequest("Andre", 0, 600));

        Assert.Empty(errors);
    }

    [Fact]
    public void KillsAtTheLimit_AreAccepted()
    {
        var errors = ScoreValidator.Validate(new SubmitScoreRequest("Andre", 50, 10));

        Assert.Empty(errors);
    }

    [Fact]
    public void KillsAboveTheLimit_AreRejected()
    {
        var errors = ScoreValidator.Validate(new SubmitScoreRequest("Andre", 51, 10));

        Assert.True(errors.ContainsKey("kills"));
    }

    [Fact]
    public void ZeroSeconds_AllowsAFewKills()
    {
        var errors = ScoreValidator.Validate(new SubmitScoreRequest("Andre", 2, 0));

        Assert.Empty(errors);
    }
}