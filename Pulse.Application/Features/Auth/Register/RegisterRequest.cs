using System;
using System.Collections.Generic;
using System.Text;

namespace Pulse.Application.Features.Auth.Register
{
    public sealed record RegisterRequest(
        string Email,
        string Password,
        string DisplayName
        );
}
