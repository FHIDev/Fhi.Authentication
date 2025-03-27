using Duende.AspNetCore.Authentication.JwtBearer.DPoP;
using Microsoft.AspNetCore.Authorization;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "bearer";
})
    .AddJwtBearer("bearer", options =>
    {
        options.Audience = "fhi:weather";
        options.Authority = "https://helseid-sts.test.nhn.no";
    })
    .AddJwtBearer("dpop", options =>
    {
        options.Audience = "fhi:weather";
        options.Authority = "https://helseid-sts.test.nhn.no";
    });

builder.Services.ConfigureDPoPTokensForScheme("dpop");

builder.Services.AddTransient<IWeatherForcastService, WeatherForcastService>();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
    options.AddPolicy("dpop", policy =>
    {
        policy.AuthenticationSchemes.Add("dpop");
        policy.RequireAuthenticatedUser();
    });
});


//builder.Services.AddAuthorization(o =>
//{
//    o.FallbackPolicy = new AuthorizationPolicyBuilder()
//                .RequireAuthenticatedUser()
//                .Build();
//});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


public static class DPoPExtensions
{
    private const string DPoPAuthorizationSchema = "DPoP ";

    public static bool TryGetDPoPAccessToken(this HttpRequest request, out string token)
    {
        token = "";
        var authorization = request.Headers.Authorization.SingleOrDefault() ?? "";
        if (string.IsNullOrEmpty(authorization) || !authorization.StartsWith(DPoPAuthorizationSchema, System.StringComparison.InvariantCultureIgnoreCase))
        {
            return false;
        }

        token = authorization.Substring(DPoPAuthorizationSchema.Length);
        return true;
    }
}
