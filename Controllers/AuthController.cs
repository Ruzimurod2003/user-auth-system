using Microsoft.AspNetCore.Mvc;
using UserAuthSystem.Models;
using UserAuthSystem.Services;

namespace UserAuthSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IJwtTokenService _jwtTokenService;

    public AuthController(IJwtTokenService jwtTokenService)
    {
        _jwtTokenService = jwtTokenService;
    }
    
    [HttpPost("login")]
    public IActionResult Login(User user)
    {
        var token = _jwtTokenService.GenerateToken(user.Id, user.Email, "User");
        return Ok(new { Token = token });
    }
}
