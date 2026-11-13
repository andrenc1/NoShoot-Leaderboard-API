using NoShoot.Api.Security;

namespace NoShoot.Api.Tests;

public class PlayerTokenTests
{
    [Fact]
    public void Generate_Returns64HexCharacters()
    {
        string token = PlayerToken.Generate();

        Assert.Equal(64, token.Length);
        Assert.Matches("^[0-9A-F]+$", token);
    }

    [Fact]
    public void Generate_ReturnsDifferentTokensEachTime()
    {
        string first = PlayerToken.Generate();
        string second = PlayerToken.Generate();

        Assert.NotEqual(first, second);
    }

    [Fact]
    public void Hash_SameToken_ReturnsSameHash()
    {
        string token = PlayerToken.Generate();

        Assert.Equal(PlayerToken.Hash(token), PlayerToken.Hash(token));
    }

    [Fact]
    public void Hash_DifferentTokens_ReturnDifferentHashes()
    {
        Assert.NotEqual(PlayerToken.Hash("token-a"), PlayerToken.Hash("token-b"));
    }

    [Fact]
    public void Hash_IsNotTheTokenItself()
    {
        string token = PlayerToken.Generate();

        Assert.NotEqual(token, PlayerToken.Hash(token));
    }
}