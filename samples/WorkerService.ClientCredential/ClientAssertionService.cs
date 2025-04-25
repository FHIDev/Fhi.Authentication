using Duende.AccessTokenManagement;
using Duende.IdentityModel;
using Duende.IdentityModel.Client;
using Fhi.Authentication.Tokens;
using Microsoft.Extensions.Options;

namespace WorkerService.ClientCredential
{
    internal class ClientAssertionService : IClientAssertionService
    {
        private readonly IOptionsMonitor<ClientCredentialsClient> _options;
        private readonly ClientConfiguration _clientConfiguration;
        private readonly IClientAssertionTokenHandler _clientAssertionTokenService;

        public ClientAssertionService(
            IOptionsMonitor<ClientCredentialsClient> options,
            IOptions<ClientConfiguration> clientConfigurations,
            IClientAssertionTokenHandler clientAssertionTokenService)
        {
            _options = options;
            _clientConfiguration = clientConfigurations.Value;
            _clientAssertionTokenService = clientAssertionTokenService;
        }
        public async Task<ClientAssertion?> GetClientAssertionAsync(string? clientName = null, TokenRequestParameters? parameters = null)
        {
            var client = new HttpClient();
            //var options = _options.Get(clientName);

            //Get issuer and token endpoint from discovery document
            var discovery = await client.GetDiscoveryDocumentAsync(_clientConfiguration.Authority);
            var jwt = _clientAssertionTokenService.CreateJwtToken(discovery.Issuer!, _clientConfiguration.ClientId, _clientConfiguration.PrivateJwk);

            return new ClientAssertion
            {
                Type = OidcConstants.ClientAssertionTypes.JwtBearer,
                Value = jwt
            };
        }
    }
}
