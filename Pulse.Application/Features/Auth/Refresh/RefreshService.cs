using Pulse.Application.Features.Auth.Login;
using Pulse.Application.Interfaces;
using Pulse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulse.Application.Features.Auth.Refresh
{
    public class RefreshService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITokenHasher _tokenHasher;
        private readonly ITokenService _tokenService;

        public RefreshService(IRefreshTokenRepository refreshTokenRepository, IUserRepository userRepository, ITokenHasher tokenHasher, ITokenService tokenService)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _userRepository = userRepository;
            _tokenHasher = tokenHasher;
            _tokenService = tokenService;
        }

        public async Task<LoginResponse> RefreshAsync(RefreshRequest request, CancellationToken cancellationToken) {
            var tokenHash = _tokenHasher.Hash(request.RefreshToken);

            var refreshToken = await _refreshTokenRepository.GetByTokenHashAsync(
                tokenHash,
                cancellationToken);

            if (refreshToken is null)
            {
                throw new UnauthorizedAccessException("Invalid refresh token");
            }

            if (refreshToken.IsRevoked || refreshToken.IsExpired)
            {
                throw new UnauthorizedAccessException("Invalid refresh token");
            }

            var user = await _userRepository.GetByIdAsync(
                refreshToken.UserId,
                cancellationToken);

            if (user is null)
            {
                throw new UnauthorizedAccessException("Invalid refresh token");
            }

            var accessToken = _tokenService.GenerateAccessToken(user);

            var newRefreshToken = _tokenService.GenerateRefreshToken();

            var newRefreshTokenHash = _tokenHasher.Hash(newRefreshToken.Token);

            var newRefreshTokenEntity = new RefreshToken(
                user.Id,
                newRefreshTokenHash,
                newRefreshToken.ExpiresAt);

            refreshToken.Revoke();

            await _refreshTokenRepository.RevokeAsync(refreshToken.Id, cancellationToken);

            return new LoginResponse(
                accessToken.Token,
                newRefreshToken.Token,
                accessToken.ExpiresAt);
        }
    }
}
