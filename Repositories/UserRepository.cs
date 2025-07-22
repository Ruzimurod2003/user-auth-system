using UserAuthSystem.Models;
using UserAuthSystem.Services;

namespace UserAuthSystem.Repositories;

public interface IUserRepository
{
    User GetUserByRefreshToken(string refreshToken);
    void UpdateUser(User user);
    List<User> GetAllUsers();
    bool UserExists(string email);
    void AddUser(User user);
}
public class UserRepository : IUserRepository
{
    private readonly IJwtTokenService _jwtTokenService;
    public List<User> _users { get; set; }
    public UserRepository(IJwtTokenService jwtTokenService)
    {
        _jwtTokenService = jwtTokenService;
        _users = new List<User>()
        {        
            new User {
                Id = 1,
                Email = "ruzimurodabdunazarov2003@mail.ru",
                FullName = "Ruzimurod Abdunazarov",
                RoleId = 1,
                Password = _jwtTokenService.HashPassword("paSsw0rd@123$"),
                RefreshToken = _jwtTokenService.GenerateRefreshToken(),
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new User {
                Id = 2,
                Email = "ruzimurodabdunazarov@gmail.com",
                FullName = "Ruzimurod Abdunazarov",
                RoleId = 2,
                Password = _jwtTokenService.HashPassword("paSsw0rd@123$"),
                RefreshToken = _jwtTokenService.GenerateRefreshToken(),
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };
    }

    public User GetUserByRefreshToken(string refreshToken)
    {
        return _users.SingleOrDefault(u => u.RefreshToken == refreshToken);
    }

    public void UpdateUser(User user)
    {
        var index = _users.FindIndex(u => u.Id == user.Id);
        if (index != -1)
        {
            _users[index] = user;
        }
    }

    public List<User> GetAllUsers()
    {
        return _users;
    }

    public bool UserExists(string email)
    {
        return _users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }

    public void AddUser(User user)
    {
        user.Id = _users.Max(u => u.Id) + 1;
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        _users.Add(user);
    }
}
