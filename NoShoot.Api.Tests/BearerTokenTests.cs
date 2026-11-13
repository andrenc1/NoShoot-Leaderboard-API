using NoShoot.Api.Security;

namespace NoShoot.Api.Tests;

public class BearerTokenTests
{
    [Theory]
    [InlineData("Bearer abc123", "abc123")]
    [InlineData("bearer abc123", "abc123")]
    [InlineData("Bearer   abc123  ", "abc123")]
    public void Read_ValidHeader_ReturnsToken(string header, string expected)
    {
        Assert.Equal(expected, BearerToken.Read(header));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("abc123")]
    [InlineData("Basic abc123")]
    [InlineData("Bearer ")]
    [InlineData("Bearer    ")]
    public void Read_InvalidHeader_ReturnsNull(string? header)
    {
        Assert.Null(BearerToken.Read(header));
    }
}