using Microsoft.AspNetCore.Mvc;
using UserAuthSystem.Models;

namespace UserAuthSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    [HttpGet]
    public IActionResult GetUsers()
    {
        var users = new List<User>
        {
            new User {
                Id = 1,
                Email = "ruzimurodabdunazarov2003@mail.ru",
                FullName = "Ruzimurod Abdunazarov",
                RoleId = 1,
                PasswordHash = "hashed_password_sample",
                RefreshToken = "refresh_token_sample",
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new User {
                Id = 2,
                Email = "ruzimurodabdunazarov@gmail.com",
                FullName = "Ruzimurod Abdunazarov",
                RoleId = 2,
                PasswordHash = "hashed_password_sample_2",
                RefreshToken = "refresh_token_sample_2",
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };
        return Ok(users);
    }
}
