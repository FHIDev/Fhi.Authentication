using BlazorServerWebApp.Components;
using BlazorServerWebApp.Hosting;
using BlazorServerWebApp.Services;
using Duende.AccessTokenManagement.OpenIdConnect;
using Fhi.Authentication.OpenIdConnect;
using Fhi.Authentication.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

internal static class WebApplicationExtensions
{
    internal static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        var authSettingsSection = builder.Configuration.GetSection("HelseIdAuthentication");
        builder.Services.Configure<AuthenticationSettings>(authSettingsSection);
        var helseIdAuthenticationSettings = authSettingsSection.Get<AuthenticationSettings>();

        var duendeSettingsSection = builder.Configuration.GetSection("DuendeAuthentication");
        builder.Services.Configure<AuthenticationSettings>(duendeSettingsSection);
        var duendeAuthenticationSettings = duendeSettingsSection.Get<AuthenticationSettings>();

        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = "duende";
            options.DefaultSignOutScheme = "duende";
        }).AddCookie(options =>
        {
            options.Cookie.Name = "BlazorSample";
        })
        .AddOpenIdConnect("duende", options =>
        {
            options.Authority = duendeAuthenticationSettings?.Authority;
            options.ClientId = duendeAuthenticationSettings?.ClientId;
            options.ClientSecret = duendeAuthenticationSettings?.ClientSecret;
            options.CallbackPath = "/signin-oidc";
            options.ResponseType = "code";
            options.PushedAuthorizationBehavior = PushedAuthorizationBehavior.Disable;

            options.Events.OnTokenValidated = async context =>
            {
                var userTokenStore = context.HttpContext.RequestServices.GetRequiredService<IUserTokenStore>();

                var exp = DateTimeOffset.UtcNow.AddSeconds(double.Parse(context.TokenEndpointResponse!.ExpiresIn));
                await userTokenStore.StoreTokenAsync(context.Principal!, new UserToken
                {
                    AccessToken = context.TokenEndpointResponse.AccessToken,
                    AccessTokenType = context.TokenEndpointResponse.TokenType,
                    Expiration = exp,
                    RefreshToken = context.TokenEndpointResponse.RefreshToken,
                    Scope = context.TokenEndpointResponse.Scope
                });

            };

            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.Scope.Add("offline_access");
            options.Scope.Add("fhi:webapi/weather");
            options.SaveTokens = true;
            options.GetClaimsFromUserInfoEndpoint = true;
            options.MapInboundClaims = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = "sub",
                RoleClaimType = "role"
            };

            options.ClaimActions.MapJsonKey("sub", "sub");
        })
        .AddOpenIdConnect("helseid", options =>
        {
            options.Authority = helseIdAuthenticationSettings?.Authority;
            options.ClientId = helseIdAuthenticationSettings?.ClientId;
            options.ClientSecret = helseIdAuthenticationSettings?.ClientSecret;
            options.Events.OnAuthorizationCodeReceived = context => context.AuthorizationCodeReceivedWithClientAssertion();
            options.Events.OnPushAuthorization = context => context.PushAuthorizationWithClientAssertion();

            options.CallbackPath = "/signin-oidc";
            options.ResponseType = "code";
            options.GetClaimsFromUserInfoEndpoint = true;
            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("helseid://scopes/identity/pid");
            options.Scope.Add("helseid://scopes/identity/security_level");

            options.SaveTokens = true;
            options.MapInboundClaims = false;
            options.TokenValidationParameters.NameClaimType = "sub";
        });

        builder.Services.AddOpenIdConnectAccessTokenManagement()
           .AddBlazorServerAccessTokenManagement<ServerSideTokenStore>();
        builder.Services.AddTransient<IClientAssertionTokenHandler, DefaultClientAssertionTokenHandler>();


        builder.Services.AddUserAccessTokenHttpClient("WebApi", configureClient: client =>
        {
            client.BaseAddress = new Uri("https://localhost:7150");
        });

        builder.Services.AddTransient<WeatherService>();
        builder.Services.AddRazorPages();
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<TokenService>();



        return builder;
    }

    internal static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.MapGet("/login", async (HttpContext context) =>
        {
            var scheme = context.Request.Query["scheme"];
            var returnUrl = context.Request.Query["returnUrl"].ToString() ?? "/";

            if (string.IsNullOrEmpty(scheme))
            {
                context.Response.Redirect(returnUrl);
                return;
            }

            await context.ChallengeAsync(scheme, new AuthenticationProperties { RedirectUri = returnUrl });
        });

        app.MapGet("/logout", async (HttpContext context) =>
        {
            var scheme = context.Request.Query["scheme"];
            var returnUrl = context.Request.Query["returnUrl"].ToString() ?? "/";

            if (string.IsNullOrEmpty(scheme))
            {
                context.Response.Redirect(returnUrl);
                return;
            }

            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await context.SignOutAsync(scheme, new AuthenticationProperties { RedirectUri = returnUrl });
        });


        app.UseStaticFiles();

        app.UseRouting();
        app.UseAntiforgery();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        return app;
    }
}
