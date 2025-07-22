using Dapper;
using Npgsql;
using UserAuthSystem.DTOs;
using UserAuthSystem.Extensions;
using UserAuthSystem.Models;

namespace UserAuthSystem.Repositories;

public interface IUserRepository
{
    User GetUserByRefreshToken(string refreshToken);
    void UpdateUser(User user);
    List<User> GetAllUsers();
    User GetUserByEmail(string email);
    User GetUserById(int id);
    bool UserExists(string email);
    void AddUser(RegisterRequestDTO user);
    void DeleteUser(User user);
}

public class UserRepository : IUserRepository
{
    private readonly IConfiguration _configuration;

    public UserRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private NpgsqlConnection CreateConnection()
    {
        return new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
    }

    public User GetUserByRefreshToken(string refreshToken)
    {
        using var connection = CreateConnection();
        string sql = "SELECT * FROM users WHERE refresh_token = @RefreshToken LIMIT 1";
        return connection.QueryFirstOrDefault<User>(sql, new { RefreshToken = refreshToken });
    }

    public void UpdateUser(User user)
    {
        using var connection = CreateConnection();
        string sql = @"
            UPDATE users SET
                email = @Email,
                full_name = @FullName,
                role = @Role,
                password = @Password,
                refresh_token = @RefreshToken,
                refresh_token_expiry = @RefreshTokenExpiry,
                updated_at = @UpdatedAt
            WHERE id = @Id";
        connection.Execute(sql, user);
    }

    public List<User> GetAllUsers()
    {
        using var connection = CreateConnection();
        return connection.Query<User>("SELECT * FROM users").ToList();
    }

    public bool UserExists(string email)
    {
        using var connection = CreateConnection();
        string sql = "SELECT COUNT(1) FROM users WHERE LOWER(email) = LOWER(@Email)";
        return connection.ExecuteScalar<bool>(sql, new { Email = email });
    }

    public void AddUser(RegisterRequestDTO userDTO)
    {
        using var connection = CreateConnection();
        string sql = @"
            INSERT INTO users (email, full_name, role, password, refresh_token, refresh_token_expiry, created_at, updated_at)
            VALUES (@Email, @FullName, @Role, @Password, @RefreshToken, @RefreshTokenExpiry, @CreatedAt, @UpdatedAt)";

        var newUser = new User
        {
            Email = userDTO.Email,
            FullName = userDTO.FullName,
            Role = "User",
            Password = PasswordExtension.HashPassword(userDTO.Password),
            RefreshToken = TokenExtension.GenerateRefreshToken(),
            RefreshTokenExpiry = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        connection.Execute(sql, newUser);
    }

    public void DeleteUser(User user)
    {
        using var connection = CreateConnection();
        string sql = "DELETE FROM users WHERE id = @Id";
        connection.Execute(sql, new { user.Id });
    }
    public User GetUserByEmail(string email)
    {
        using var connection = CreateConnection();
        string sql = "SELECT * FROM users WHERE LOWER(email) = LOWER(@Email) LIMIT 1";
        return connection.QueryFirstOrDefault<User>(sql, new { Email = email });
    }
    public User GetUserById(int id)
    {
        using var connection = CreateConnection();
        string sql = "SELECT * FROM users WHERE id = @Id LIMIT 1";
        return connection.QueryFirstOrDefault<User>(sql, new { Id = id });
    }
}
