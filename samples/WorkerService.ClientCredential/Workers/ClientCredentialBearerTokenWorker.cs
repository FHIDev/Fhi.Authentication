using Duende.AccessTokenManagement;
using Duende.IdentityModel;
using Duende.IdentityModel.Client;
using Fhi.Authentication.Tokens;
using Microsoft.Extensions.Options;

namespace WorkerService.Workers
{
    /// <summary>
    /// This worker is used to create bearer token using client credentials flow. It contains sample of using HttpClient extension from duende and using HttpRequest
    /// </summary>
    internal class ClientCredentialBearerTokenWorker : BackgroundService
    {
        private readonly ILogger<ClientCredentialBearerTokenWorker> _logger;
        private readonly IHttpClientFactory _factory;
        private readonly IClientCredentialsTokenManagementService _clientCredentialsTokenManagement;
        private readonly ClientConfiguration _clientConfigurations;

        public ClientCredentialBearerTokenWorker(
            ILogger<ClientCredentialBearerTokenWorker> logger,
            IHttpClientFactory factory,
            IOptions<ClientConfiguration> clientConfigurations,
            IClientCredentialsTokenManagementService clientCredentialsTokenManagement)
        {
            _logger = logger;
            _factory = factory;
            _clientCredentialsTokenManagement = clientCredentialsTokenManagement;
            _clientConfigurations = clientConfigurations.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            /************************************************************************************************
            * Manually getting token and set authorization header on the API request. 
            * **********************************************************************************************/
            using var client = new HttpClient();
            //Get issuer and token endpoint from discovery document
            var discovery = await client.GetDiscoveryDocumentAsync(_clientConfigurations.Authority);
            if (discovery is not null && !discovery.IsError && discovery.Issuer is not null && discovery.TokenEndpoint is not null)
            {
                var tokenRequest = new ClientCredentialsTokenRequest()
                {
                    ClientId = _clientConfigurations.ClientId,
                    Address = discovery.TokenEndpoint,
                    GrantType = OidcConstants.GrantTypes.ClientCredentials,
                    ClientCredentialStyle = ClientCredentialStyle.PostBody,
                    ////ClientSecret = _clientConfigurations.Secret, // This is used for shared secrets, not needed when using client assertion
                    ClientAssertion = new ClientAssertion()
                    {
                        Type = OidcConstants.ClientAssertionTypes.JwtBearer,
                        Value = ClientAssertionTokenHandler.CreateJwtToken(discovery.Issuer, _clientConfigurations.ClientId, _clientConfigurations.Secret)
                    },
                    Scope = _clientConfigurations.Scope
                };
                var tokenResponse = await client.RequestClientCredentialsTokenAsync(tokenRequest);
                _logger.LogInformation("Bearer token response: " + tokenResponse.Raw);

                //API request with bearer token
                var apiRequest = new HttpRequestMessage(HttpMethod.Get, new Uri(new Uri("https://localhost:7150"), "api/v1/integration/health-records"));
                apiRequest.SetBearerToken(tokenResponse.AccessToken!);
                var healthRecordApiResponse = await client.SendAsync(apiRequest);
                _logger.LogInformation("Bearer api response: " + await healthRecordApiResponse.Content.ReadAsStringAsync());
            }

            /************************************************************************************************
             * Manually getting token using duende service from AccessTokenManagement package.
             * This method is used by the HttpClient extensions from Duende. 
            /************************************************************************************************/
            var duendeTokenResponse = await _clientCredentialsTokenManagement.GetAccessTokenAsync(
                _clientConfigurations.ClientName,
                new TokenRequestParameters()
                {
                    Assertion = new ClientAssertion()
                    {
                        Type = OidcConstants.ClientAssertionTypes.JwtBearer,
                        Value = ClientAssertionTokenHandler.CreateJwtToken(discovery!.Issuer!, _clientConfigurations.ClientId, _clientConfigurations.Secret)
                    },
                    Scope = _clientConfigurations.Scope
                });
            _logger.LogInformation("Duende manual response: " + duendeTokenResponse.AccessToken);

            /************************************************************************************************
            * Using registered HttpClient with Duende Http Delegation handler to create bearer token. 
            * See service configuration
            * **********************************************************************************************/
            var healthApiClient = _factory.CreateClient(_clientConfigurations.ClientName);
            var response = await healthApiClient.GetAsync("api/v1/integration/health-records");
            _logger.LogInformation("Using HttpClient Response: " + await response.Content.ReadAsStringAsync());

            //Should not have access to end-user endpoint
            var meResponse = await healthApiClient.GetAsync("api/v1/me/health-records");
            if (meResponse.IsSuccessStatusCode)
                throw new Exception("User should not have access");
            _logger.LogInformation("Access denied response: " + meResponse.StatusCode);

            await RunService(cancellationToken);
        }

        private async Task RunService(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(1000, cancellationToken);
            }
        }
    }
}
