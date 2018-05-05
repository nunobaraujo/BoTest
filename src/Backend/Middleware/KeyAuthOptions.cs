using Microsoft.AspNetCore.Authentication;

namespace Backend.Middleware
{
    public class KeyAuthOptions : AuthenticationSchemeOptions
    {
        public const string DefaultHeaderName = "api-key";
        //public const string TokenHeaderName = "session-token";
        public const string AuthenticationScheme = "Automatic";
    }
}
