namespace UserAuthSystem.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; } // Bu yerda porolni Encrypt() qilingani bilan ishlayveraman
    public string FullName { get; set; }
    public int RoleId { get; set; }
    public string RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
