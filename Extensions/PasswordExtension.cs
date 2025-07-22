using System.Security.Cryptography;
using System.Text;

namespace UserAuthSystem.Extensions;

public static class PasswordExtension
{
    public static string HashPassword(this string password)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            var hashedBytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

            var hashedPassword = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

            return hashedPassword;
        }
    }

    public static bool VerifyPassword(this string password, string hashedPassword)
    {
        return password.HashPassword() == hashedPassword;
    }
}
