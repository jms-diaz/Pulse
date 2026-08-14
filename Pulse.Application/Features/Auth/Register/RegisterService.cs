using Pulse.Application.Interfaces;
using Pulse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulse.Application.Features.Auth.Register
{
    public class RegisterService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;

        public RegisterService(IUserRepository userRepository, IPasswordService passwordService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
        }

        public async Task<User> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken) { 
            var email = request.Email.Trim().ToLowerInvariant();

            var exists = await _userRepository.ExistsByEmailAsync(email, cancellationToken);

            if (exists)
            {
                throw new InvalidOperationException("Email is already registered");
            }

            var user = new User
            {
                Email = email,
                PasswordHash = _passwordService.Hash(request.Password),
                DisplayName = request.DisplayName.Trim(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            return await _userRepository.CreateAsync(user);
        }
    }
}
