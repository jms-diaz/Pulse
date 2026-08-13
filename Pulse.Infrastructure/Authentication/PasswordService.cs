using Microsoft.AspNet.Identity;
using Pulse.Application.Interfaces;
using Pulse.Domain.Entities;

namespace Pulse.Infrastructure.Authentication
{
    public class PasswordService : IPasswordService
    {
        private readonly PasswordHasher _hasher = new();

        public string Hash(string password)
        {
            return _hasher.HashPassword(password);
        }

        public bool Verify(string password, string passwordHash)
        {
            var result =  _hasher.VerifyHashedPassword(password, passwordHash);

            return result != PasswordVerificationResult.Failed;
        }
    }
}
