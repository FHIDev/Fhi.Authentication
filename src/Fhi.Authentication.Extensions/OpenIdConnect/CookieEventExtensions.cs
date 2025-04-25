using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Globalization;

namespace Fhi.Authentication.OpenIdConnect
{
    public static class CookieEventExtensions
    {
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
        public static async Task<TokenValidateResponse> ValidateTokenExpirationAsync(this CookieValidatePrincipalContext context)
        {
            if (context.Principal?.Identity is not null && context.Principal.Identity.IsAuthenticated)
            {
                var tokens = context.Properties.GetTokens();
                var accessToken = tokens.SingleOrDefault(t => t.Name == OpenIdConnectParameterNames.AccessToken);
                var refreshToken = tokens.SingleOrDefault(t => t.Name == OpenIdConnectParameterNames.RefreshToken);

                if (accessToken == null || string.IsNullOrEmpty(accessToken.Value) || refreshToken == null || string.IsNullOrEmpty(refreshToken.Value))
                {
                    return new TokenValidateResponse(true, "NotFound", "Access token or refresh token is missing. Rejecting principal and renewing cookie.");
                }

                var expiresAt = DateTimeOffset.Parse(tokens.SingleOrDefault(t => t.Name == "expires_at")?.Value ?? string.Empty, CultureInfo.InvariantCulture);
                if (expiresAt <= DateTimeOffset.UtcNow)
                {
                    var tokenService = context.HttpContext.RequestServices.GetRequiredService<ITokenService>();
                    var refreshedTokens = await tokenService.RefreshAccessTokenAsync(refreshToken.Value);

                    if (refreshedTokens.IsError)
                    {
                        return new TokenValidateResponse(true, "ExpiredRefreshToken", "Refresh token is expired. Rejecting principal so that the user can re-authenticate");
                    }
                }

            }

            return new TokenValidateResponse(false);
        }

        public record TokenValidateResponse(bool IsError, string Error = "", string ErrorDescription = "");
    }
}
