using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    [Authorize(Roles = "Admin")]
    public IActionResult GetUsers()
    {
        return Ok(_userRepository.GetAllUsers());
    }

    [HttpDelete("delete/{id}")]
    public IActionResult DeleteUser(int id)
    {
        var user = _userRepository.GetAllUsers().SingleOrDefault(u => u.Id == id);
        if (user == null)
            return NotFound("User not found");

        _userRepository.DeleteUser(user);
        return Ok("User deleted successfully");
    }
}
