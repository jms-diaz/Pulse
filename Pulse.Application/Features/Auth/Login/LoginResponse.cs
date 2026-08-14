using System;
using System.Collections.Generic;
using System.Text;

namespace Pulse.Application.Features.Auth.Login
{
    public sealed record LoginResponse(
        string AccessToken,
        string RefreshToken,
        DateTime AccessTokenExpiresAt);
}
