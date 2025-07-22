using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using UserAuthSystem.Hubs;
using UserAuthSystem.Services;

namespace UserAuthSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IHubContext<NotificationHub> _notificationHub;

    public UserController(IUserService userService, IHubContext<NotificationHub> notificationHub)
    {
        _userService = userService;
        _notificationHub = notificationHub;
    }

    [HttpGet("users")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetUsers()
    {
        return Ok(_userService.GetAllUsers());
    }

    [HttpDelete("delete/{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult DeleteUser(int id)
    {
        var user = _userService.GetUserById(id);
        if (user == null)
            return NotFound("User not found");

        _userService.DeleteUser(id);

        _notificationHub.Clients.All.SendAsync("UserDeleted", id);

        return Ok("User deleted successfully");
    }
}
