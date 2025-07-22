using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using UserAuthSystem.Hubs;
using UserAuthSystem.Repositories;

namespace UserAuthSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IHubContext<NotificationHub> _notificationHub;

    public UserController(IUserRepository userRepository, IHubContext<NotificationHub> notificationHub)
    {
        _userRepository = userRepository;
        _notificationHub = notificationHub;
    }

    [HttpGet("users")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetUsers()
    {
        return Ok(_userRepository.GetAllUsers());
    }

    [HttpDelete("delete/{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteUser(int id)
    {
        var user = _userRepository.GetAllUsers().SingleOrDefault(u => u.Id == id);
        if (user == null)
            return NotFound("User not found");

        _userRepository.DeleteUser(user);
        
        _notificationHub.Clients.All.SendAsync("UserDeleted", user.Id);

        return Ok("User deleted successfully");
    }
}
