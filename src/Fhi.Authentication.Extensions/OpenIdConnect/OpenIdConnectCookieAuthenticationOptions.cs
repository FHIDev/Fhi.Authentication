using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Fhi.Authentication.OpenIdConnect
{
    /// <summary>
    /// Setting default values for OpenIdConnect authentication.
    /// </summary>
    public class OpenIdConnectCookieAuthenticationOptions : IPostConfigureOptions<CookieAuthenticationOptions>
    {
        /// <inheritdoc/>
        public void PostConfigure(string? name, CookieAuthenticationOptions options)
        {
            if (name == CookieAuthenticationDefaults.AuthenticationScheme)
            {
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.SlidingExpiration = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.EventsType = typeof(OpenIdConnectCookieEventsForApi);
            }
        }
    }
}