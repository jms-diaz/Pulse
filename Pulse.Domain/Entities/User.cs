using Pulse.Domain.Enums;

namespace Pulse.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public UserRole Role { get; private set; } = UserRole.Member;

        public ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();

    }
}
