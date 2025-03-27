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
        private readonly IOptions<ClientConfiguration> _clientConfiguration;
        private readonly IClientAssertionTokenHandler _clientAssertionTokenService;

        public ClientAssertionService(
            IOptionsMonitor<ClientCredentialsClient> options,
            IOptions<ClientConfiguration> clientConfigurations,
            IClientAssertionTokenHandler clientAssertionTokenService)
        {
            _options = options;
            _clientConfiguration = clientConfigurations;
            _clientAssertionTokenService = clientAssertionTokenService;
        }
        public Task<ClientAssertion?> GetClientAssertionAsync(string? clientName = null, TokenRequestParameters? parameters = null)
        {
            //var options = _options.Get(clientName);
            var jwt = _clientAssertionTokenService.CreateJwtToken("https://helseid-sts.test.nhn.no",
                _clientConfiguration.Value.ClientId,
                _clientConfiguration.Value.PrivateJwk);

            return Task.FromResult<ClientAssertion?>(new ClientAssertion
            {
                Type = OidcConstants.ClientAssertionTypes.JwtBearer,
                Value = jwt
            });
        }
    }
}
