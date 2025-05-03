using Microsoft.AspNetCore.Authentication;

namespace Fhi.Auth.IntegrationTests
{
    public partial class Tests
    {
        public class FakeAuthHandler : AuthenticationHandler<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions>
        {
            public static IEnumerable<System.Security.Claims.Claim> TestClaims { get; set; } = [];

            public FakeAuthHandler(
                Microsoft.Extensions.Options.IOptionsMonitor<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions> options,
                Microsoft.Extensions.Logging.ILoggerFactory logger,
                System.Text.Encodings.Web.UrlEncoder encoder)
                : base(options, logger, encoder) { }

            protected override Task<AuthenticateResult> HandleAuthenticateAsync()
            {
                var identity = new System.Security.Claims.ClaimsIdentity(TestClaims, "Fake");
                var principal = new System.Security.Claims.ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, "Fake");
                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
        }
    }
}
