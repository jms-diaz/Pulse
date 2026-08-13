using Pulse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulse.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> ExistsByEmailAsync(string email);
        Task<User> CreateAsync(User user);
    }
}
