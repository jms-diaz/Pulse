using Microsoft.Extensions.Options;
using Pulse.Application.Interfaces;
using Pulse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulse.Application.Features.Auth.Login
{
    public sealed class LoginService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;
        private readonly ITokenHasher _tokenHasher;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public LoginService(IUserRepository userRepository, IPasswordService passwordService, ITokenService tokenService, ITokenHasher tokenHasher, IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _tokenService = tokenService;
            _tokenHasher = tokenHasher;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken) { 
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

            if (user is null)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            var passwordValid = _passwordService.Verify(request.Password, user.PasswordHash);

            if (!passwordValid)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            var accessToken = _tokenService.GenerateAccessToken(user);

            var refreshToken = _tokenService.GenerateRefreshToken();

            var refreshTokenHash = _tokenHasher.Hash(refreshToken.Token);

            var refreshTokenEntity = new RefreshToken(
                user.Id,
                refreshTokenHash,
                refreshToken.ExpiresAt);

            await _refreshTokenRepository.CreateAsync(
                refreshTokenEntity,
                cancellationToken);

            return new LoginResponse(
                accessToken.Token,
                refreshToken.Token,
                accessToken.ExpiresAt);
        }
    }
}
