using Pulse.Application.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Pulse.Api.Endpoints.Auth
{
    public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
    {
        private ClaimsPrincipal User =>
            httpContextAccessor.HttpContext?.User
            ?? new ClaimsPrincipal();

        public bool IsAuthenticated =>
            User.Identity?.IsAuthenticated ?? false;

        public int UserId =>
            int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? throw new InvalidOperationException("User ID claim is missing."));

        public string Email =>
            User.FindFirstValue(ClaimTypes.Email)
                ?? throw new InvalidOperationException("Email claim is missing.");
    }
}
