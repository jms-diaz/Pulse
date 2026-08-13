using Dapper;
using Npgsql;
using Pulse.Application.Interfaces;
using Pulse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulse.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly NpgsqlDataSource _dataSource;

        public UserRepository(NpgsqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<User> CreateAsync(User user)
        {
            const string sql =
                """
                INSERT INTO users
                    (email, password_hash, display_name, created_at, updated_at)
                VALUES
                    (@Email, @PasswordHash, @DisplayName, @CreatedAt, @UpdatedAt)
                RETURNING
                    id AS Id,
                    email AS Email,
                    password_hash AS PasswordHash,
                    display_name AS DisplayName,
                    created_at AS CreatedAt,
                    updated_at AS UpdatedAt;
                """;

            await using var connection = await _dataSource.OpenConnectionAsync();

            return await connection.QuerySingleAsync<User>(
                sql,
                user);
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            const string sql = 
                """
                SELECT EXISTS (
                    SELECT 1
                    FROM users
                    WHERE email = @Email
                )
                """;

            await using var connection = await _dataSource.OpenConnectionAsync();

            return await connection.ExecuteScalarAsync<bool>(
                sql,
                new { Email = email });
        }
    }
}
