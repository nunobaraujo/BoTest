using Backend.Middleware.Validator;
using Core.Services.Session;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Backend.Middleware
{
    public class KeyAuthHandler : AuthenticationHandler<KeyAuthOptions>
    {
        private readonly IApiKeyValidator _apiKeyValidator;
        private readonly ISessionService _sessionService;

        public KeyAuthHandler(IOptionsMonitor<KeyAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IApiKeyValidator apiKeyValidator, ISessionService sessionService)
            : base(options, logger, encoder, clock)
        {
            _apiKeyValidator = apiKeyValidator;
            _sessionService = sessionService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Context.Request.Headers.TryGetValue(KeyAuthOptions.DefaultHeaderName, out var headerValue))
            {
                return AuthenticateResult.Fail("No api key header.");
            }

            var apiKey = headerValue.First();
            if (!_apiKeyValidator.Validate(apiKey))
            {
                return AuthenticateResult.Fail("Invalid API key.");
            }

            //if (!Context.Request.Headers.TryGetValue(KeyAuthOptions.TokenHeaderName, out var tokenValue))
            //{
            //    return AuthenticateResult.Fail("No token header.");
            //}
            //string userId;
            //try { userId = await _sessionService.GetUserId(tokenValue); }
            //catch { userId = null; }
            //if (userId == null)
            //    return AuthenticateResult.Fail("Invalid token.");

            var identity = new ClaimsIdentity("apikey");
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), null, "apikey");
            return await Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
