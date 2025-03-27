using Duende.AccessTokenManagement;
using Fhi.Authentication.Tokens;
using WorkerService.ClientCredential;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddTransient<IClientAssertionService, ClientAssertionService>();
builder.Services.AddTransient<IClientAssertionTokenHandler, DefaultClientAssertionTokenHandler>();

builder.Services.AddDistributedMemoryCache();

//register token management for clients
var clientConfiguration = new ClientConfiguration();
builder.Configuration.GetSection("ClientConfiguration").Bind(clientConfiguration);
builder.Services.Configure<ClientConfiguration>(builder.Configuration.GetSection("ClientConfiguration"));
builder.Services
    .AddClientCredentialsTokenManagement()
    //Client with Bearer
    .AddClient(clientConfiguration.ClientName, options =>
    {
        options.TokenEndpoint = clientConfiguration.TokenEndpoint;
        options.ClientId = clientConfiguration.ClientId;
        options.Scope = clientConfiguration.Scope;
    })
    //Client with DPoP
    .AddClient(clientConfiguration.ClientName + ".dpop", options =>
    {
        options.TokenEndpoint = clientConfiguration.TokenEndpoint;
        options.ClientId = clientConfiguration.ClientId;
        options.Scope = clientConfiguration.Scope;
        //Can use client assertion key or generate a new
        options.DPoPJsonWebKey = clientConfiguration.PrivateJwk;
    });

// Register HTTP client
builder.Services.AddTransient<LoggingHandler>();
builder.Services.AddClientCredentialsHttpClient("weatherapi", clientConfiguration.ClientName, client =>
{
    client.BaseAddress = new Uri("https://localhost:7150");
}).AddHttpMessageHandler<LoggingHandler>();

builder.Services.AddClientCredentialsHttpClient("weatherapi.dpop", clientConfiguration.ClientName + ".dpop", client =>
{
    client.BaseAddress = new Uri("https://localhost:7150");
}).AddHttpMessageHandler<LoggingHandler>();

var host = builder.Build();
host.Run();
