namespace NoShoot.Api.Security;

public static class BearerToken
{
    private const string Prefix = "Bearer ";

    public static string? Read(string? authorizationHeader)
    {
        if (authorizationHeader is null)
            return null;

        if (!authorizationHeader.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase))
            return null;

        string token = authorizationHeader[Prefix.Length..].Trim();

        return token.Length == 0 ? null : token;
    }
}