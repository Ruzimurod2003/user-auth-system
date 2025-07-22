using Dapper;
using Npgsql;
using UserAuthSystem.Services;

namespace UserAuthSystem.Repositories;

public class DatabaseInitializer
{
    private readonly IConfiguration _configuration;
    private readonly IJwtTokenService _jwtTokenService;

    public DatabaseInitializer(IConfiguration configuration, IJwtTokenService jwtTokenService)
    {
        _configuration = configuration;
        _jwtTokenService = jwtTokenService;
    }

    private NpgsqlConnection CreateConnection()
    {
        return new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
    }

    public async Task EnsureDatabaseCreatedAsync()
    {
        var createTableSql = @"
            CREATE TABLE IF NOT EXISTS users (
                id SERIAL PRIMARY KEY,
                email TEXT NOT NULL UNIQUE,
                full_name TEXT NOT NULL,
                role TEXT NOT NULL CHECK (role IN ('User', 'Admin')),
                password TEXT NOT NULL,
                refresh_token TEXT,
                refresh_token_expiry TIMESTAMP,
                created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
                updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
            );
        ";

        var createFunctionSql = @"
            CREATE OR REPLACE FUNCTION update_updated_at_column()
            RETURNS TRIGGER AS $$
            BEGIN
                NEW.updated_at = CURRENT_TIMESTAMP;
                RETURN NEW;
            END;
            $$ LANGUAGE plpgsql;
        ";

        var createTriggerSql = @"
            DO $$
            BEGIN
                IF NOT EXISTS (
                    SELECT 1 FROM pg_trigger WHERE tgname = 'update_users_updated_at'
                ) THEN
                    CREATE TRIGGER update_users_updated_at
                    BEFORE UPDATE ON users
                    FOR EACH ROW
                    EXECUTE FUNCTION update_updated_at_column();
                END IF;
            END;
            $$;
        ";

        using var connection = CreateConnection();
        await connection.ExecuteAsync(createTableSql);
        await connection.ExecuteAsync(createFunctionSql);
        await connection.ExecuteAsync(createTriggerSql);

        
        var checkAdminSql = "SELECT COUNT(1) FROM users WHERE email = @Email";
        var adminEmail = "admin@example.com";

        var adminExists = await connection.ExecuteScalarAsync<bool>(checkAdminSql, new { Email = adminEmail });

        if (!adminExists)
        {
            var seedSql = @"
                INSERT INTO users (
                    email, full_name, role, password, refresh_token, refresh_token_expiry, created_at, updated_at
                )
                VALUES (
                    @Email, @FullName, @Role, @Password, @RefreshToken, @RefreshTokenExpiry, @CreatedAt, @UpdatedAt
                );
            ";

            var now = DateTime.UtcNow;
            var seedUser = new
            {
                Email = adminEmail,
                FullName = "Admin User",
                Role = "Admin",
                Password = _jwtTokenService.HashPassword("Admin@123!"),
                RefreshToken = _jwtTokenService.GenerateRefreshToken(),
                RefreshTokenExpiry = now.AddDays(7),
                CreatedAt = now,
                UpdatedAt = now
            };

            await connection.ExecuteAsync(seedSql, seedUser);
        }
    }
}
