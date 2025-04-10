using Duende.AccessTokenManagement.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Globalization;

public class CookieEvents : CookieAuthenticationEvents
{
    private readonly IUserTokenEndpointService _userTokenEndpointService;

    public CookieEvents(IUserTokenEndpointService userTokenEndpointService)
    {
        _userTokenEndpointService = userTokenEndpointService;
    }

    /// <summary>
    /// Only needed if you have an downstream API. 
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        if (context.Principal?.Identity is not null && context.Principal.Identity.IsAuthenticated)
        {
            var tokens = context.Properties.GetTokens();
            var accessToken = tokens.SingleOrDefault(t => t.Name == OpenIdConnectParameterNames.AccessToken);

            if (accessToken == null || string.IsNullOrEmpty(accessToken.Value))
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                context.ShouldRenew = true;
                return;
            }

            var expiresAt = tokens.SingleOrDefault(t => t.Name == "expires_at");
            var dtExpires = DateTimeOffset.Parse(expiresAt.Value, CultureInfo.InvariantCulture);
            if (dtExpires <= DateTimeOffset.UtcNow)
            {
                var refreshToken = tokens.SingleOrDefault(t => t.Name == OpenIdConnectParameterNames.RefreshToken);
                if (refreshToken == null || string.IsNullOrEmpty(refreshToken.Value))
                {
                    context.RejectPrincipal();
                    context.ShouldRenew = true;
                    return;
                }

                var refreshedTokens = await _userTokenEndpointService.RefreshAccessTokenAsync(new UserToken()
                {
                    RefreshToken = refreshToken.Value
                }, new UserTokenRequestParameters());

                if (refreshedTokens.IsError)
                {
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

