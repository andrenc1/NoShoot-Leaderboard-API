using NoShoot.Api.Validation;

namespace NoShoot.Api.Tests;

public class PlayerNameValidatorTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Abc")]
    [InlineData("Abcdefghijklmnopqrstu")]
    [InlineData("<script>")]
    [InlineData("André")]
    [InlineData("Ana Silva")]
    [InlineData("Ana-Maria")]
    public void InvalidName_IsRejected(string? name)
    {
        var errors = PlayerNameValidator.Validate(name);

        Assert.True(errors.ContainsKey("name"));
    }

    [Theory]
    [InlineData("Abcd")]
    [InlineData("Abcdefghijklmnopqrst")]
    [InlineData("andrenc1")]
    [InlineData("Player_1")]
    [InlineData("ANDRE_2026")]
    [InlineData(" andrenc1 ")]
    public void ValidName_IsAccepted(string name)
    {
        var errors = PlayerNameValidator.Validate(name);

        Assert.Empty(errors);
    }

    [Theory]
    [InlineData("andrenc1", "ANDRENC1")]
    [InlineData("AndreNC1", "ANDRENC1")]
    [InlineData("  andrenc1  ", "ANDRENC1")]
    public void Normalize_IgnoresCaseAndSpaces(string input, string expected)
    {
        Assert.Equal(expected, PlayerNameValidator.Normalize(input));
    }
}