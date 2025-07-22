using Microsoft.AspNetCore.Mvc;
using UserAuthSystem.DTOs;
using UserAuthSystem.Extensions;
using UserAuthSystem.Services;

namespace UserAuthSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IConfiguration _configuration;

    public AuthController(IJwtTokenService jwtTokenService, IUserService userService, IConfiguration configuration)
    {
        _jwtTokenService = jwtTokenService;
        _userService = userService;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequestDTO userDTO)
    {
        if (_userService.UserExists(userDTO.Email))
            return BadRequest("User already exists");

        _userService.RegisterUser(userDTO);
        return Ok("User registered successfully");
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequestDTO model)
    {
        var user = _userService.AuthenticateUser(model.Email, model.Password);
        if (user == null)
            return Unauthorized("Invalid email or password");

        var token = _jwtTokenService.GenerateToken(user.Id, user.Email, user.Role);
        var refreshToken = TokenExtension.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(_configuration.GetValue<int>("Jwt:Expire"));

        _userService.UpdateUser(user);

        return Ok(new { Token = token, RefreshToken = refreshToken });
    }

    [HttpPost("refresh-token")]
    public IActionResult RefreshToken([FromBody] TokenRequestDTO model)
    {
        var user = _userService.GetUserByRefreshToken(model.RefreshToken);
        if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
            return Unauthorized("Invalid or expired refresh token");

        var newToken = _jwtTokenService.GenerateToken(user.Id, user.Email, user.Role);
        var newRefreshToken = TokenExtension.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(_configuration.GetValue<int>("Jwt:Expire"));

        _userService.UpdateUser(user);

        return Ok(new { Token = newToken, RefreshToken = newRefreshToken });
    }
}
