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

        public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
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

            await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

            return await connection.QuerySingleAsync<User>(
                new CommandDefinition(
                    sql,
                    user,
                    cancellationToken: cancellationToken));
        }

        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            const string sql = 
                """
                SELECT EXISTS (
                    SELECT 1
                    FROM users
                    WHERE email = @Email
                )
                """;

            await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

            return await connection.ExecuteScalarAsync<bool>(
                new CommandDefinition(
                    sql,
                    new { Email = email },
                    cancellationToken: cancellationToken));
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            const string sql =
                """
                SELECT 
                    id, 
                    email, 
                    password_hash AS PasswordHash, 
                    display_name AS DisplayName,
                    created_at AS CreatedAt,
                    updated_at AS UpdatedAt
                FROM users
                WHERE email = @Email
                """;

            await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

            return await connection.QuerySingleOrDefaultAsync<User>(
                new CommandDefinition(
                    sql,
                    new { Email = email },
                    cancellationToken: cancellationToken));
        }
    }
}
