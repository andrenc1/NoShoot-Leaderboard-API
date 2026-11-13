using System.Security.Cryptography;
using System.Text;

namespace NoShoot.Api.Security;

public static class PlayerToken
{
    public static string Generate() =>
        Convert.ToHexString(RandomNumberGenerator.GetBytes(32));

    public static string Hash(string token) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}