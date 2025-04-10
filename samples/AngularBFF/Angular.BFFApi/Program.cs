using AngularBFF.Net8.Api.Weather;
using Duende.AccessTokenManagement.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();

var authSettingsSection = builder.Configuration.GetSection("Authentication");
builder.Services.Configure<AuthenticationSettings>(authSettingsSection);
var authenticationSettings = authSettingsSection.Get<AuthenticationSettings>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
}).AddCookie(options =>
{
    options.Cookie.Name = "angular";
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    //Should be set to a value before refresh token expirers
    options.ExpireTimeSpan = TimeSpan.FromSeconds(45);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.EventsType = typeof(CookieEvents);
})
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.Authority = authenticationSettings!.Authority;
    options.ClientId = authenticationSettings.ClientId;
    options.ClientSecret = authenticationSettings.ClientSecret;
    options.CallbackPath = "/signin-oidc";
    options.ResponseType = "code";
    options.PushedAuthorizationBehavior = PushedAuthorizationBehavior.Disable;

    options.Events.OnRedirectToIdentityProvider = context =>
    {
        if (!context.Request.Path.StartsWithSegments("/login"))
        {
            context.Response.Headers["Location"] = context.ProtocolMessage.CreateAuthenticationRequestUrl();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.HandleResponse();
        }

        return Task.CompletedTask;
    };

    options.Scope.Clear();
    options.Scope.Add("openid");
    options.Scope.Add("offline_access");
    options.Scope.Add("fhi:webapi/weather");
    options.GetClaimsFromUserInfoEndpoint = true;
    options.MapInboundClaims = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = "sub",
    };
    options.SaveTokens = true;
});

builder.Services.AddOpenIdConnectAccessTokenManagement(options =>
{
    options.RefreshBeforeExpiration = TimeSpan.FromSeconds(10);
    options.ChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
});
builder.Services.AddTransient<CookieEvents>();

builder.Services.AddTransient<RefreshTokenFailureHandler>();
builder.Services.AddUserAccessTokenHttpClient(
    "WebApi",
    parameters: new UserTokenRequestParameters()
    {
        ChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme,
    },
    configureClient: (provider, client) =>
    {
        client.BaseAddress = new Uri("https://localhost:7150");
    }).AddHttpMessageHandler<RefreshTokenFailureHandler>();

builder.Services.AddTransient<IWeatherForecastService, WeatherForecastService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/**Require authentication on all requests***/
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/login", [AllowAnonymous] async (HttpContext context) =>
{
    if (context.User is null || context.User.Identity is null || !context.User.Identity.IsAuthenticated)
    {
        await context.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties { RedirectUri = "/" });
    }

    //var returnUrl = context.Request.Query["returnUrl"].ToString();
    //if (string.IsNullOrEmpty(returnUrl))
    //{
    //    returnUrl = "/";
    //}

});

app.MapGet("/session", (HttpContext context) =>
{
    if (context.User?.Identity?.IsAuthenticated == true)
    {
        return Results.Ok(new { isAuthenticated = true, isError = false });
    }

    return Results.Ok(new { isAuthenticated = false, isError = false });
});


app.MapGet("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    await context.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
});

app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();

