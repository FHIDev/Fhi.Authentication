using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Fhi.Auth.IntegrationTests.Setup
{
    internal static class WebApplicationBuilderTestHost
    {
        internal static WebApplicationBuilder CreateWebHostBuilder()
        {
            var builder = WebApplication.CreateBuilder([]);
            builder.WebHost.UseTestServer();

            return builder;
        }

        internal static WebApplicationBuilder WithConfiguration(this WebApplicationBuilder builder, IConfiguration configuration)
        {
            builder.Configuration.AddConfiguration(configuration);
            return builder;
        }

        internal static WebApplicationBuilder WithServices(this WebApplicationBuilder builder, Action<IServiceCollection> services)
        {
            services.Invoke(builder.Services);
            return builder;
        }

        internal static WebApplication BuildApp(this WebApplicationBuilder builder, Action<WebApplication> appBuilder)
        {
            var app = builder.Build();
            appBuilder.Invoke(app);
            return app;
        }


    }

    public class ConfigureCookieAuthenticationOptions : IConfigureNamedOptions<CookieAuthenticationOptions>
    {
        public void Configure(string? name, CookieAuthenticationOptions options)
        {
            Configure(options);
        }

        public void Configure(CookieAuthenticationOptions options)
        {
            options.CookieManager = new FakeCookieManager();
            options.TicketDataFormat = new FakeCookieTicketDataFormat();
        }
    }

    internal class FakeCookieManager : ICookieManager
    {
        public void AppendResponseCookie(HttpContext context, string key, string? value, CookieOptions options)
        {
            throw new NotImplementedException();
        }

        public void DeleteCookie(HttpContext context, string key, CookieOptions options)
        {
            throw new NotImplementedException();
        }

        public string GetRequestCookie(HttpContext context, string key)
        {
            var task = "";
            return task;
        }
    }

    internal class FakeCookieTicketDataFormat : ISecureDataFormat<AuthenticationTicket>
    {
        public string Protect(AuthenticationTicket data)
        {
            throw new NotImplementedException();
        }

        public string Protect(AuthenticationTicket data, string? purpose)
        {
            throw new NotImplementedException();
        }

        public AuthenticationTicket? Unprotect(string? protectedText)
        {
            //var jwt = new JsonWebToken(protectedText);
            //var claimsIdentity = new ClaimsIdentity(jwt.Claims, "scheme");
            //AuthenticationTicket ticket = claimsIdentity.CreateAuthenticationTicket("scheme", null);

            //return ticket;
            return new AuthenticationTicket(new ClaimsPrincipal(), "scheme");
        }

        public AuthenticationTicket? Unprotect(string? protectedText, string? purpose)
        {
            return Unprotect(protectedText);
        }
    }
}
