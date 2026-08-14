using System;
using System.Collections.Generic;
using System.Text;

namespace Pulse.Domain.Entities
{
    public sealed class RefreshToken
    {
        public int Id { get; private set; }
        public int UserId { get; private set; }
        public string TokenHash { get; private set; } = null!;
        public DateTime ExpiresAt { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? RevokedAt { get; private set; }
        public User User { get; private set; } = null!;
        public RefreshToken()
        {
        }

        public RefreshToken(
            int userId,
            string tokenHash,
            DateTime expiresAt)
        {
            UserId = userId;
            TokenHash = tokenHash;
            ExpiresAt = expiresAt;
            CreatedAt = DateTime.UtcNow;
        }


        public void Revoke()
        {
            RevokedAt = DateTime.UtcNow;
        }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

        public bool IsRevoked => RevokedAt.HasValue;
    }
}
