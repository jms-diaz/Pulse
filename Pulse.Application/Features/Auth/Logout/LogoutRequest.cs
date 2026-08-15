using System;
using System.Collections.Generic;
using System.Text;

namespace Pulse.Application.Features.Auth.Logout
{
    public sealed record LogoutRequest(string RefreshToken);
}
