using Duende.IdentityModel;
using Duende.IdentityModel.Client;
using Fhi.Authentication.Tokens;
using Microsoft.Extensions.Options;
using WorkerService.ClientCredential;

namespace WorkerService.Workers
{
    /// <summary>
    /// This worker is used to create bearer token using client credentials flow. It contains sample of using HttpClient extension from duende and using HttpRequest
    /// </summary>
    internal class ClientCredentialBearerTokenWorker : BackgroundService
    {
        private readonly ILogger<ClientCredentialBearerTokenWorker> _logger;
        private readonly IHttpClientFactory _factory;
        private readonly IClientAssertionTokenHandler _clientAssertionTokenHandler;
        private readonly ClientConfiguration _clientConfigurations;

        public ClientCredentialBearerTokenWorker(
            ILogger<ClientCredentialBearerTokenWorker> logger,
            IHttpClientFactory factory,
            IClientAssertionTokenHandler clientAssertionTokenHandler,
            IOptions<ClientConfiguration> clientConfigurations
            )
        {
            _logger = logger;
            _factory = factory;
            _clientAssertionTokenHandler = clientAssertionTokenHandler;
            _clientConfigurations = clientConfigurations.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            /************************************************************************************************
            * Manually getting token and set authorization header on the API request. 
            * **********************************************************************************************/
            var client = new HttpClient();
            //Get issuer and token endpoint from discovery document
            var discovery = await client.GetDiscoveryDocumentAsync(_clientConfigurations.Authority);
            if (discovery is not null && !discovery.IsError && discovery.Issuer is not null && discovery.TokenEndpoint is not null)
            {
                //Get token using client credentials flow
                var tokenRequest = new ClientCredentialsTokenRequest()
                {
                    ClientId = _clientConfigurations.ClientId,
                    Address = discovery.TokenEndpoint,
                    GrantType = OidcConstants.GrantTypes.ClientCredentials,
                    ClientCredentialStyle = ClientCredentialStyle.PostBody,
                    ClientAssertion = new ClientAssertion()
                    {
                        Type = OidcConstants.ClientAssertionTypes.JwtBearer,
                        Value = _clientAssertionTokenHandler.CreateJwtToken(discovery.Issuer, _clientConfigurations.ClientId, _clientConfigurations.PrivateJwk)
                    },
                    Scope = _clientConfigurations.Scope
                };
                var tokenResponse = await client.RequestClientCredentialsTokenAsync(tokenRequest);
                _logger.LogInformation("Bearer token response: " + tokenResponse.Raw);

                //API request with bearer token
                var apiRequest = new HttpRequestMessage(HttpMethod.Get, new Uri(new Uri("https://localhost:7150"), "api/v1/integration/health-records"));
                apiRequest.SetBearerToken(tokenResponse.AccessToken!);
                var weatherApiResponse = await client.SendAsync(apiRequest);
                _logger.LogInformation("Bearer weather response: " + await weatherApiResponse.Content.ReadAsStringAsync());
            }

            /************************************************************************************************
            * Using registered HttpClient with Duende Http Delegation handler to create bearer token. 
            * See service configuration
            * **********************************************************************************************/
            var weatherApiClient = _factory.CreateClient("health-records");
            var response = await weatherApiClient.GetAsync("api/v1/integration/health-records");
            _logger.LogInformation("Dpop weather response: " + await response.Content.ReadAsStringAsync());

            //Should not have access to end-user endpoint
            var meResponse = await weatherApiClient.GetAsync("api/v1/me/health-records");

            await RunService(stoppingToken);
        }

        private async Task RunService(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
