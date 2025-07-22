using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace UserAuthSystem.Services;

public interface IJwtTokenService
{
    string GenerateToken(int userId, string email, string role);
}

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _config;

    public JwtTokenService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateToken(int userId, string email, string role)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role)
        };

        var authSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["JWT:Key"]));

        var token = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"],
            expires: DateTime.UtcNow.AddMinutes(_config.GetValue<int>("Jwt:Expire")),
            claims: claims,
            signingCredentials: new SigningCredentials(
                key: authSigningKey,
                algorithm: SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
