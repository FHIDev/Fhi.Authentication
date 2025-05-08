using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

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
}
