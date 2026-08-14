using Pulse.Application.Features.Auth.Login;
using Pulse.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pulse.Application.Interfaces
{
    public interface ITokenService
    {
        AccessTokenResult GenerateAccessToken(User user);
        string GenerateRefreshToken();
    }
}
