using System;
using System.Collections.Generic;
using System.Text;

namespace Pulse.Application.Interfaces
{
    public interface IPasswordService
    {
        string Hash(string password);
        bool Verify(string password, string passwordHash);
    }
}
