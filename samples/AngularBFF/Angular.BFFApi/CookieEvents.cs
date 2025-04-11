using Duende.AccessTokenManagement.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Globalization;

namespace Angular.BFFApi
{
    public class CookieEvents : CookieAuthenticationEvents
    {
        private readonly IUserTokenEndpointService _userTokenEndpointService;
        private readonly ILogger<CookieEvents> _logger;

        public CookieEvents(IUserTokenEndpointService userTokenEndpointService, ILogger<CookieEvents> logger)
        {
            _userTokenEndpointService = userTokenEndpointService;
            _logger = logger;
        }

        /// <summary>
        /// This override is required when using downstream APIs that rely on access tokens.
        /// 
        /// The authentication cookie may remain valid even after the access and refresh tokens have expired.
        /// In such cases, the user will still appear as logged in (based on the cookie), but calls to downstream APIs 
        /// will fail with a 401 Unauthorized error because the tokens are no longer valid.
        /// 
        /// This method ensures that the refresh token is still valid. If it is expired, the principal is rejected,
        /// prompting the authentication system to renew the cookie (and potentially trigger a re-login).
        /// 
        /// Note: This mechanism must be used in combination with a configured <c>ExpireTimeSpan</c> for the cookie,
        /// to align cookie lifetime management with token lifetimes.
        /// </summary>
        /// <param name="context">The context for validating the authentication principal.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            if (context.Principal?.Identity is not null && context.Principal.Identity.IsAuthenticated)
            {
                var tokens = context.Properties.GetTokens();
                var accessToken = tokens.SingleOrDefault(t => t.Name == OpenIdConnectParameterNames.AccessToken);
                var refreshToken = tokens.SingleOrDefault(t => t.Name == OpenIdConnectParameterNames.RefreshToken);

                if (accessToken == null || string.IsNullOrEmpty(accessToken.Value) || refreshToken == null || string.IsNullOrEmpty(refreshToken.Value))
                {
                    _logger.LogError("Access token or refresh token is missing. Rejecting principal and renewing cookie.");
                    context.RejectPrincipal();
                    context.ShouldRenew = true;
                    return;
                }

                var expiresAt = DateTimeOffset.Parse(tokens.SingleOrDefault(t => t.Name == "expires_at")?.Value ?? string.Empty, CultureInfo.InvariantCulture);
                if (expiresAt <= DateTimeOffset.UtcNow)
                {
                    _logger.LogInformation("Access token has expired. Attempting to refresh it using the refresh token.");
                    var refreshedTokens = await _userTokenEndpointService.RefreshAccessTokenAsync(
                        new UserToken()
                        {
                            RefreshToken = refreshToken.Value
                        },
                        new UserTokenRequestParameters());

                    if (refreshedTokens.IsError)
                    {
                        _logger.LogWarning("Refresh token is expired. Rejecting principal so that the user can re-authenticate");
                        context.RejectPrincipal();
                        context.ShouldRenew = true;
                        return;
                    }
                }
            }

            await base.ValidatePrincipal(context);
        }

        public override async Task SigningOut(CookieSigningOutContext context)
        {
            await context.HttpContext.RevokeRefreshTokenAsync();

            await base.SigningOut(context);
        }
    }
}