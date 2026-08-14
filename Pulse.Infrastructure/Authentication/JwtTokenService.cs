using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Pulse.Application.Features.Auth.Login;
using Pulse.Application.Interfaces;
using Pulse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Pulse.Infrastructure.Authentication
{
    public sealed class JwtTokenService(IOptions<JwtOptions> options) : ITokenService
    {
        private readonly JwtOptions _options = options.Value;

        public AccessTokenResult GenerateAccessToken(User user)
        {
            // Claims identify the user and provide information used by the API.
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(JwtRegisteredClaimNames.Name, user.DisplayName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Convert the secret key into cryptographic key used to sign the JWT
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_options.SecretKey));

            // Create the JWT's signature
            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var expiresAt = DateTime.UtcNow.AddMinutes(_options.AccessTokenExpirationInMinutes);

            // Create the JWT with its issuer, audience, claims, and expiration
            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    _options.AccessTokenExpirationInMinutes),
                signingCredentials: credentials);

            // Serialize the JWT object into a string
            var tokenString = new JwtSecurityTokenHandler()
                .WriteToken(token);

            return new AccessTokenResult(tokenString, expiresAt);
        }

        public string GenerateRefreshToken()
        {
            // Generate secure random value used to refresh access tokens
            return Convert.ToBase64String(
                RandomNumberGenerator.GetBytes(64));
        }
    }
}
