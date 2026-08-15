using Dapper;
using Npgsql;
using Pulse.Application.Interfaces;
using Pulse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulse.Infrastructure.Persistence.Repositories
{
    public sealed class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly NpgsqlDataSource _dataSource;

        public RefreshTokenRepository(NpgsqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task CreateAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
        {
            const string sql = 
                """
                INSERT INTO refresh_tokens
                    (user_id, token_hash, expires_at, created_at, revoked_at)
                VALUES
                    (@UserId, @TokenHash, @ExpiresAt, @CreatedAt, @RevokedAt);
                """;

            await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

            await connection.ExecuteAsync(
                new CommandDefinition(
                    sql,
                    refreshToken,
                    cancellationToken: cancellationToken));
        }

        public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
        {
            const string sql = 
                """
                SELECT
                    id AS Id,
                    user_id AS UserId,
                    token_hash AS TokenHash,
                    expires_at AS ExpiresAt,
                    created_at AS CreatedAt,
                    revoked_at AS RevokedAt
                FROM refresh_tokens
                WHERE token_hash = @TokenHash;
                """;

            await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
            
            return await connection.QuerySingleOrDefaultAsync<RefreshToken>(
                new CommandDefinition(
                    sql,
                    new {TokenHash = tokenHash },
                    cancellationToken: cancellationToken));
        }

        public async Task RevokeAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
        {
            const string sql =
                """
                UPDATE refresh_tokens
                SET revoked_at = @RevokedAt
                WHERE id = @Id;
                """;

            await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

            await connection.ExecuteAsync(
                new CommandDefinition(
                    sql,
                    new
                    {
                        Id = refreshToken.Id,
                        RevokedAt = refreshToken.RevokedAt
                    },
                    cancellationToken: cancellationToken));
        }

        public async Task RotateAsync(RefreshToken currentToken, RefreshToken newToken ,CancellationToken cancellationToken = default)
        {
            await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

            await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

            try
            {
                const string revokeSql =
                """
                UPDATE refresh_tokens
                SET revoked_at = @RevokedAt
                WHERE id = @Id;
                """;

                await connection.ExecuteAsync(
                    new CommandDefinition(
                        revokeSql,
                        new
                        {
                            Id = currentToken.Id,
                            RevokedAt = currentToken.RevokedAt
                        },
                        transaction: transaction,
                        cancellationToken: cancellationToken));

                const string insertSql =
                """
                INSERT INTO refresh_tokens
                    (user_id, token_hash, expires_at, created_at)
                VALUES
                    (@UserId, @TokenHash, @ExpiresAt, @CreatedAt);
                """;

                await connection.ExecuteAsync(
                    new CommandDefinition(
                        insertSql,
                        new
                        {
                            newToken.UserId,
                            newToken.TokenHash,
                            newToken.ExpiresAt, 
                            newToken.CreatedAt
                        },
                        transaction: transaction,
                        cancellationToken: cancellationToken));

                await transaction.CommitAsync(cancellationToken);

            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
