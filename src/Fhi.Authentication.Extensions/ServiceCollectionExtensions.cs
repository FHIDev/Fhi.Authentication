using BlazorInteractiveServer.Hosting.Authentication;
using Fhi.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Fhi.Authentication
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the FHI authentication services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The service collection to add the authentication services to.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddOpenIdConnectCookieEventServices(this IServiceCollection services)
        {
            services.AddTransient<OpenIdConnectCookieEventsForApi>();
            services.AddTransient<ITokenService, DefaultTokenService>();
            services.AddSingleton<IPostConfigureOptions<CookieAuthenticationOptions>, OpenIdConnectCookieAuthenticationOptions>();

            return services;
        }
    }
}
