using System.Security.Cryptography;

namespace UserAuthSystem.Extensions;

public static class TokenExtension
{
    public static string GenerateRefreshToken()
    {
        var random = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(random);
        return Convert.ToBase64String(random);
    }
}
