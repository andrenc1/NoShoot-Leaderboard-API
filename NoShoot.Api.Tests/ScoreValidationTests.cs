using NoShoot.Api.Contracts;
using NoShoot.Api.Validation;

namespace NoShoot.Api.Tests;

public class ScoreValidatorTests
{
    [Fact]
    public void ValidRequest_HasNoErrors()
    {
        var request = new SubmitScoreRequest(62, 50);

        var errors = ScoreValidator.Validate(request);

        Assert.Empty(errors);
    }

    [Fact]
    public void NegativeKills_IsRejected()
    {
        var errors = ScoreValidator.Validate(new SubmitScoreRequest(-1, 60));

        Assert.True(errors.ContainsKey("kills"));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(601)]
    public void SecondsOutOfRange_IsRejected(int seconds)
    {
        var errors = ScoreValidator.Validate(new SubmitScoreRequest(0, seconds));

        Assert.True(errors.ContainsKey("survivedSeconds"));
    }

    [Fact]
    public void FullMatch_IsAccepted()
    {
        var errors = ScoreValidator.Validate(new SubmitScoreRequest(0, 600));

        Assert.Empty(errors);
    }

    [Fact]
    public void KillsAtTheLimit_AreAccepted()
    {
        var errors = ScoreValidator.Validate(new SubmitScoreRequest(50, 10));

        Assert.Empty(errors);
    }

    [Fact]
    public void KillsAboveTheLimit_AreRejected()
    {
        var errors = ScoreValidator.Validate(new SubmitScoreRequest(51, 10));

        Assert.True(errors.ContainsKey("kills"));
    }

    [Fact]
    public void ZeroSeconds_AllowsAFewKills()
    {
        var errors = ScoreValidator.Validate(new SubmitScoreRequest(2, 0));

        Assert.Empty(errors);
    }
}