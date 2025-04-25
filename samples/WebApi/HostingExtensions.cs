using Fhi.Authorization;
using Microsoft.AspNetCore.Authorization;
using WebApi.Authorization;
using WebApi.Services;

namespace WebApi
{
    public static class HostingExtensions
    {
        public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            /********************************************************************************************************
            * Authentication
            ********************************************************************************************************/
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "bearer.me";
                options.DefaultChallengeScheme = "bearer.me";
            })
                .AddJwtBearer("bearer.me", options =>
                {
                    options.Audience = "fhi:webapi";
                    options.Authority = "https://localhost:5001/";
                })
                .AddJwtBearer("bearer.integration", options =>
                {
                    options.Audience = "fhi:webapi";
                    options.Authority = "https://localhost:5001/";
                });

            builder.Services.AddTransient<IHealthRecordService, HealthRecordService>();

            /********************************************************************************************************
             * Authorization
             ********************************************************************************************************/
            builder.Services.AddSingleton<IAuthorizationHandler, ScopeHandler>();
            builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, DefaultAccessControlMiddleware>();

            builder.Services.AddAuthorizationBuilder()
                .SetFallbackPolicy(new AuthorizationPolicyBuilder()
                            .RequireAuthenticatedUser()
                            .Build())
                .AddPolicy("EndUserPolicy", policy =>
                {
                    policy.AuthenticationSchemes.Add("bearer.me");
                    policy.RequireClaim("scope", "fhi:webapi/access");
                    policy.RequireAuthenticatedUser();
                })
                 .AddPolicy("Integration", policy =>
                 {
                     policy.AuthenticationSchemes.Add("bearer.integration");
                     policy.RequireAuthenticatedUser();
                 });


            return builder.Build();
        }

        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();


            return app;
        }
    }
}
