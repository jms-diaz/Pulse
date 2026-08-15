using System;
using System.Collections.Generic;
using System.Text;

namespace Pulse.Application.Interfaces
{
    public interface ICurrentUser
    {
        int UserId { get; }
        string Email { get; }
        bool IsAuthenticated { get; }
    }
}
