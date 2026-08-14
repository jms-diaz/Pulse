using System;
using System.Collections.Generic;
using System.Text;

namespace Pulse.Application.Features.Auth.Login
{
    public sealed record AccessTokenResult(
        string Token,
        DateTime ExpiresAt);
}
