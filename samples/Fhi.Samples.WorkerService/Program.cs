using Duende.AccessTokenManagement;
using WorkerService;
using WorkerService.Workers;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<ClientCredentialDPoPTokenWorker>();
builder.Services.AddHostedService<ClientCredentialBearerTokenWorker>();

builder.Services.AddTransient<IClientAssertionService, ClientCredentialAssertionService>();

builder.Services.AddDistributedMemoryCache();

//register token management for Http clients
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
        options.DPoPJsonWebKey = clientConfiguration.Secret;
    });

// Register HTTP client
builder.Services.AddTransient<LoggingHandler>();
builder.Services.AddClientCredentialsHttpClient(clientConfiguration.ClientName, clientConfiguration.ClientName, client =>
{
    client.BaseAddress = new Uri("https://localhost:7150");
}).AddHttpMessageHandler<LoggingHandler>();

builder.Services.AddClientCredentialsHttpClient(clientConfiguration.ClientName + ".dpop", clientConfiguration.ClientName + ".dpop", client =>
{
    client.BaseAddress = new Uri("https://localhost:7150");
}).AddHttpMessageHandler<LoggingHandler>();

var host = builder.Build();
host.Run();
