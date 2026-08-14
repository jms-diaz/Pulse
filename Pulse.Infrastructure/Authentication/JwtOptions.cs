using System;
using System.Collections.Generic;
using System.Text;

namespace Pulse.Infrastructure.Authentication
{
    public sealed class JwtOptions
    {
        public const string SectionName = "Jwt";
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public required string SecretKey { get; set; }
        public int AccessTokenExpirationInMinutes{ get; set; }

    }
}
