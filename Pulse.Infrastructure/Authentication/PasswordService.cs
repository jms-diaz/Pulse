using Microsoft.AspNetCore.Identity;
using Pulse.Application.Interfaces;

namespace Pulse.Infrastructure.Authentication
{
    public sealed class PasswordService : IPasswordService
    {
        private readonly PasswordHasher<object> _hasher = new();

        public string Hash(string password)
        {
            return _hasher.HashPassword(
                new object(),
                password);
        }

        public bool Verify(string password, string passwordHash)
        {
            var result = _hasher.VerifyHashedPassword(
                new object(),
                passwordHash,
                password);

            return result is
                PasswordVerificationResult.Success or
                PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}
