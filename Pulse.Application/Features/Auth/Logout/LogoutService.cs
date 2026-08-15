using Pulse.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulse.Application.Features.Auth.Logout
{
    public class LogoutService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ITokenHasher _tokenHasher;

        public LogoutService(IRefreshTokenRepository refreshTokenRepository, ITokenHasher tokenHasher)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _tokenHasher = tokenHasher;
        }

        public async Task LogoutAsync(LogoutRequest request, CancellationToken cancellationToken) { 
            var tokenHash = _tokenHasher.Hash(request.RefreshToken);

            var refreshToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash, cancellationToken);

            if (refreshToken is null || refreshToken.IsRevoked)
            {
                return;
            }

            refreshToken.Revoke();

            await _refreshTokenRepository.RevokeAsync(refreshToken, cancellationToken);
        }
    }
}
