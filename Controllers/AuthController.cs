using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using UserAuthSystem.DTOs;
using UserAuthSystem.Models;
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

        userDTO.Password = _jwtTokenService.HashPassword(userDTO.Password);
        _userRepository.AddUser(userDTO);

        return Ok("User registered successfully");
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequestDTO model)
    {
        var user = _userRepository.GetAllUsers().SingleOrDefault(u => u.Email == model.Email);
        if (user == null || !_jwtTokenService.VerifyPassword(model.Password, user.Password))
            return Unauthorized("Invalid email or password");

        var token = _jwtTokenService.GenerateToken(user.Id, user.Email, "User");
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

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

        var newAccessToken = _jwtTokenService.GenerateToken(user.Id, user.Email, "User");
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        return Ok(new
        {
            Token = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }
}
