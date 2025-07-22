using UserAuthSystem.DTOs;
using UserAuthSystem.Extensions;
using UserAuthSystem.Models;
using UserAuthSystem.Repositories;

namespace UserAuthSystem.Services;

public interface IUserService
{
    bool UserExists(string email);
    void RegisterUser(RegisterRequestDTO userDTO);
    User AuthenticateUser(string email, string password);
    User GetUserByRefreshToken(string refreshToken);
    void UpdateUser(User user);
    List<User> GetAllUsers();
    User GetUserById(int id);
    void DeleteUser(int id);
}

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public bool UserExists(string email)
    {
        return _userRepository.UserExists(email);
    }

    public void RegisterUser(RegisterRequestDTO userDTO)
    {
        _userRepository.AddUser(userDTO);
    }

    public User AuthenticateUser(string email, string password)
    {
        var user = _userRepository.GetUserByEmail(email);
        if (user == null || !PasswordExtension.VerifyPassword(password, user.Password))
            return null;

        return user;
    }

    public User GetUserByRefreshToken(string refreshToken)
    {
        return _userRepository.GetUserByRefreshToken(refreshToken);
    }

    public void UpdateUser(User user)
    {
        _userRepository.UpdateUser(user);
    }

    public List<User> GetAllUsers()
    {
        return _userRepository.GetAllUsers();
    }

    public User GetUserById(int id)
    {
        return _userRepository.GetUserById(id);
    }

    public void DeleteUser(int id)
    {
        var user = _userRepository.GetUserById(id);
        if (user != null)
            _userRepository.DeleteUser(user);
    }
}
