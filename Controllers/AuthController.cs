using Microsoft.AspNetCore.Mvc;
using UserAuthSystem.DTOs;
using UserAuthSystem.Extensions;
using UserAuthSystem.Repositories;
using UserAuthSystem.Services;

namespace UserAuthSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUserRepository _userRepository;

    public AuthController(IJwtTokenService jwtTokenService, IUserRepository userRepository)
    {
        _jwtTokenService = jwtTokenService;
        _userRepository = userRepository;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequestDTO userDTO)
    {
        if (_userRepository.UserExists(userDTO.Email))
            return BadRequest("User already exists");

        userDTO.Password = PasswordExtension.HashPassword(userDTO.Password);
        _userRepository.AddUser(userDTO);

        return Ok("User registered successfully");
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequestDTO model)
    {
        var user = _userRepository.GetAllUsers().SingleOrDefault(u => u.Email == model.Email);
        if (user == null || !PasswordExtension.VerifyPassword(model.Password, user.Password))
            return Unauthorized("Invalid email or password");

        var token = _jwtTokenService.GenerateToken(user.Id, user.Email, user.Role);
        var refreshToken = TokenExtension.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        return Ok(new { Token = token, RefreshToken = refreshToken });
    }
    
    [HttpPost("refresh-token")]
    public IActionResult RefreshToken([FromBody] TokenRequestDTO model)
    {
        var user = _userRepository.GetUserByRefreshToken(model.RefreshToken);

        if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
            return Unauthorized("Invalid or expired refresh token");

        var newAccessToken = _jwtTokenService.GenerateToken(user.Id, user.Email, user.Role);
        var newRefreshToken = TokenExtension.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        return Ok(new
        {
            Token = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }
}
