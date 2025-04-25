using BlazorInteractiveServer.Components;
using BlazorInteractiveServer.Hosting.Authentication;
using BlazorInteractiveServer.Services;
using BlazorInteractiveServerWebApp.Services;
using Duende.AccessTokenManagement.OpenIdConnect;
using Fhi.Authentication.OpenIdConnect;
using Fhi.Authentication.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;

internal static partial class Startup
{
    internal static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        var authenticationSettingsSection = builder.Configuration.GetSection("Authentication");
        builder.Services.Configure<AuthenticationSettings>(authenticationSettingsSection);
        var authenticationSettings = authenticationSettingsSection.Get<AuthenticationSettings>();

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            options.DefaultSignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
        }).AddCookie(options =>
        {
            options.Cookie.Name = "BlazorSample";
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.ExpireTimeSpan = TimeSpan.FromSeconds(30);
            options.SlidingExpiration = true;
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.EventsType = typeof(DefaultCookieEvent);
        })
        .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
        {
            options.Authority = authenticationSettings?.Authority;
            options.ClientId = authenticationSettings?.ClientId;
            options.ClientSecret = authenticationSettings?.ClientSecret;
            options.CallbackPath = "/signin-oidc";
            options.ResponseType = "code";
            options.EventsType = typeof(BlazorOpenIdConnectEvents);

            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.Scope.Add("offline_access");
            options.Scope.Add("fhi:webapi/access");
            options.SaveTokens = true;
            options.GetClaimsFromUserInfoEndpoint = true;
            options.MapInboundClaims = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = "sub",
            };

            options.ClaimActions.MapJsonKey("sub", "sub");
        });
        builder.Services.AddTransient<IClientAssertionTokenHandler, DefaultClientAssertionTokenHandler>();
        builder.Services.AddTransient<DefaultCookieEvent>();
        builder.Services.AddTransient<BlazorOpenIdConnectEvents>();
        builder.Services.AddTransient<ITokenService, DefaultTokenService>();

        /**************************************************************************************************************************************************
        * Handling downstream API call with token handling. Since Blazor uses SignalR tokens are not available through httpcontext. Tokens must be stored *
        * in in another persistent secure storage available for the downstream API call                                                                   *
        /**************************************************************************************************************************************************/
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddOpenIdConnectAccessTokenManagement()
        .AddBlazorServerAccessTokenManagement<InMemoryUserTokenStore>();


        builder.Services.AddScoped<AuthenticationStateProvider, CustomRevalidatingAuthenticationStateProvider>();
        builder.Services.AddScoped<NavigationService>();
        builder.Services.AddCascadingAuthenticationState();

        builder.Services.AddScoped<HealthRecordService>();
        builder.Services.AddUserAccessTokenHttpClient(
            "WebApi",
            parameters: new UserTokenRequestParameters()
            {
                SignInScheme = OpenIdConnectDefaults.AuthenticationScheme
            },
            configureClient: client =>
            {
                client.BaseAddress = new Uri("https://localhost:7150");
            });

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddHubOptions(options =>
            {
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(5);
                options.HandshakeTimeout = TimeSpan.FromSeconds(5);
            });

        builder.Services.AddAntiforgery();

        builder.Services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
        });

        return builder;
    }


    internal static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.MapGet("/logout", async (HttpContext context) =>
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await context.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        });

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseAntiforgery();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        return app;
    }
}
