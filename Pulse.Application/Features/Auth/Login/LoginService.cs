using Pulse.Application.Interfaces;
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

        public LoginService(IUserRepository userRepository, IPasswordService passwordService, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _tokenService = tokenService;
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

            return new LoginResponse(
                accessToken.Token,
                refreshToken,
                accessToken.ExpiresAt);
        }
    }
}
