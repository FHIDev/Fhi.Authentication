using Duende.IdentityModel;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Fhi.Authentication.OpenIdConnect
{
    /// <summary>
    /// Set default options for OpenIdConnect authentication if not already set.
    /// </summary>
    public class DefaultOpenIdConnectOptions : IPostConfigureOptions<OpenIdConnectOptions>
    {
        /// <inheritdoc/>
        public void PostConfigure(string? name, OpenIdConnectOptions options)
        {
            if (!options.SaveTokens)
                options.SaveTokens = true;
            if (!options.GetClaimsFromUserInfoEndpoint)
                options.GetClaimsFromUserInfoEndpoint = true;
            if (!options.MapInboundClaims)
                options.MapInboundClaims = false;
            options.TokenValidationParameters ??= new TokenValidationParameters();
            if (string.IsNullOrEmpty(options.TokenValidationParameters.NameClaimType))
            {
                options.TokenValidationParameters.NameClaimType = JwtClaimTypes.Subject;
            }
        }
    }
}
