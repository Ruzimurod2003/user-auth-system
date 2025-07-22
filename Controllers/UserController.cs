using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAuthSystem.Models;
using UserAuthSystem.Repositories;

namespace UserAuthSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet("users")]
    [Authorize]
    public IActionResult GetUsers()
    {
        return Ok(_userRepository.GetAllUsers());
    }

    [HttpGet("roles")]
    public IActionResult GetRoles()
    {
        var roles = new List<Role>
        {
            new Role { Id = 1, Name = "Admin" },
            new Role { Id = 2, Name = "User" }
        };
        return Ok(roles);
    }
}
