using BlazorServerWebApp;
using BlazorServerWebApp.Components;
using Fhi.Authentication.OpenIdConnect;
using Fhi.Authentication.Tokens;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
.AddInteractiveServerComponents();

var authSettingsSection = builder.Configuration.GetSection("Authentication");
builder.Services.Configure<AuthenticationSettings>(authSettingsSection);

var authConfig = authSettingsSection.Get<AuthenticationSettings>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
}).AddCookie()
.AddOpenIdConnect(options =>
{
    options.Authority = authConfig.Authority;
    options.ClientId = authConfig.ClientId;
    //JWK or SharedSecret
    options.ClientSecret = authConfig.ClientSecret;
    options.EventsType = typeof(DefaultOpenIdConnectEvents);
    options.CallbackPath = "/signin-oidc";
    options.ResponseType = "code";
    options.GetClaimsFromUserInfoEndpoint = true;
    options.Scope.Clear();

    options.Scope.Add("openid");
});

builder.Services.AddTransient<IClientAssertionTokenHandler, DefaultClientAssertionTokenHandler>();
builder.Services.AddTransient(typeof(DefaultOpenIdConnectEvents));


/**Require authentication on all requests***/
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
