using ClientCredential.WorkerService.Utility;
using Duende.IdentityModel.Client;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WorkerService.ClientCredential;

namespace WorkerService.Workers
{
    public class ClientCredentialDPoPTokenWorker : BackgroundService
    {
        private readonly ILogger<ClientCredentialDPoPTokenWorker> _logger;
        private readonly IHttpClientFactory _factory;
        private readonly IOptions<ClientConfiguration> _clientConfigurations;

        public ClientCredentialDPoPTokenWorker(ILogger<ClientCredentialDPoPTokenWorker> logger, IHttpClientFactory factory, IOptions<ClientConfiguration> clientConfigurations)
        {
            _logger = logger;
            _factory = factory;
            _clientConfigurations = clientConfigurations;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            /**API request with Dpop token **/
            _logger.LogInformation("*******API request with Dpop token *****");
            await SampleOfDpopTokenReqest();

            /**Sample of using Duende Http Delegation handler to create dpop token**/
            _logger.LogInformation("*******START Sample of using Duende Http Delegation handler to create dpop token *****");
            var weatherApiDopClient = _factory.CreateClient("weatherapi.dpop");
            var dpopResponse = await weatherApiDopClient.GetAsync("api/v2/integration/weatherforcasts");
            _logger.LogInformation("Dpop weather response: " + await dpopResponse.Content.ReadAsStringAsync());

            _logger.LogInformation("*******END Sample of using Duende Http Delegation handler to create dpop token *****");


            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task SampleOfDpopTokenReqest()
        {
            /**Get bearer token**/
            var client = new HttpClient();
            var discovery = await client.GetDiscoveryDocumentAsync("https://helseid-sts.test.nhn.no");

            var tokenResponse = await CreateDpopToken(
                client,
                _clientConfigurations.Value.ClientId,
                _clientConfigurations.Value.PrivateJwk,
                discovery.Issuer,
                _clientConfigurations.Value.Scope,
                discovery.TokenEndpoint);
            _logger.LogInformation("Dpop token response: " + tokenResponse.Raw);

            /**API request with bearer token**/
            var uri = new Uri(new Uri("https://localhost:7150"), "api/v2/integration/weatherforcasts");
            var requestMessage = new HttpRequestMessageBuilder()
                .Create(HttpMethod.Get, uri)
                .WithDpop(
                uri.AbsoluteUri.ToString(),
                HttpMethod.Get.ToString(),
                _clientConfigurations.Value.PrivateJwk,
                "PS512",
                tokenResponse.AccessToken)
                .Build();

            //var dpopProof = DPoPProofBuilder.CreateDPoPProof(
            //    "",
            //    HttpMethod.Get,
            //    _clientConfigurations.Value.PrivateJwk,
            //    "",
            //    accessToken: accessToken);
            //requestMessage.SetDPoPToken(tokenResponse.AccessToken, tokenResponse.);

            var weatherApiResponse = await client.SendAsync(requestMessage);
            _logger.LogInformation("Dpop weather response: " + weatherApiResponse.StatusCode.ToString());
            _logger.LogInformation("Dpop weather response: " + await weatherApiResponse.Content.ReadAsStringAsync());
        }

        private async Task<TokenResponse> CreateDpopToken(
           HttpClient client,
           string clientId,
           string privateJwk,
           string issuer,
           string scope,
           string tokenEndpoint)
        {
            var jwkKey = new JsonWebKey(privateJwk);
            var nonceRequest = new ClientCredentialRequestBuilder()
                .Create(tokenEndpoint, clientId)
                .WithDPoP(tokenEndpoint, HttpMethod.Post.ToString(), privateJwk, jwkKey.Alg)
                .WithClientAssertion(issuer, privateJwk)
                .WithScope(scope)
                .Build();
            var response = await client.RequestClientCredentialsTokenAsync(nonceRequest);

            if (response.Error == "use_dpop_nonce" && response.DPoPNonce is not null)
            {
                var tokenRequest = new ClientCredentialRequestBuilder()
                    .Create(tokenEndpoint, clientId)
                    .WithDPoPNonce(tokenEndpoint, HttpMethod.Post.ToString(), privateJwk, jwkKey.Alg, response.DPoPNonce)
                    .WithClientAssertion(issuer, privateJwk)
                    .WithScope(scope)
                    .Build();
                response = await client.RequestClientCredentialsTokenAsync(tokenRequest);
            }
            else if (response.IsError)
            {
                _logger.LogError(response.Error);
            }

            return response;
        }
    }
}
