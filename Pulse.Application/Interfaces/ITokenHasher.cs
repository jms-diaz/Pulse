using System;
using System.Collections.Generic;
using System.Text;

namespace Pulse.Application.Interfaces
{
    public interface ITokenHasher
    {
        string Hash(string token);
    }
}
