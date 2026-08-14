using System;
using System.Collections.Generic;
using System.Text;

namespace Pulse.Application.Features.Auth.Refresh
{
    public sealed record RefreshRequest(
        string RefreshToken);
}
