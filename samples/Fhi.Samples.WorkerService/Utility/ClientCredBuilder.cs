using Duende.IdentityModel;
using Duende.IdentityModel.Client;
using Microsoft.IdentityModel.Tokens;

namespace ClientCredential.WorkerService.Utility
{
    public class ClientCredentialRequestBuilder
    {
        private static ClientCredentialsTokenRequest _request = new();
        public ClientCredentialRequestBuilder Create(
            string tokenEndpoint,
            string clientId,
            string grantType = OidcConstants.GrantTypes.ClientCredentials,
            ClientCredentialStyle credentialStyle = ClientCredentialStyle.PostBody)
        {
            _request = new()
            {
                ClientId = clientId,
                Address = tokenEndpoint,
                GrantType = grantType,
                ClientCredentialStyle = credentialStyle
            };

            return this;
        }

        public ClientCredentialRequestBuilder WithDPoPNonce(string uri, string httpMethod, string privateJwk, string privateJwkAlg, string nonce)
        {
            //var dpopProofAccessToken = DPoPProofBuilder.CreateDPoPProof(
            //    uri,
            //    httpMethod,
            //    privateJwk,
            //    privateJwkAlg,
            //    nonce);

            //_request.DPoPProofToken = dpopProofAccessToken;

            return this;
        }

        public ClientCredentialRequestBuilder WithDPoP(string uri, string httpMethod, string privateJwk, string privateJwkAlg)
        {
            //var dpopProofAccessToken = DPoPProofBuilder.CreateDPoPProof(
            //    uri,
            //    httpMethod,
            //    privateJwk,
            //    privateJwkAlg);

            //_request.DPoPProofToken = dpopProofAccessToken;

            return this;
        }

        public ClientCredentialRequestBuilder WithClientAssertion(string issuer, string privateJwk, string type = OidcConstants.ClientAssertionTypes.JwtBearer)
        {
            //var clientAssertion = ClientAssertionBuilder.CreateClientAssertionJwt(issuer, _request.ClientId, privateJwk);
            //_request.ClientAssertion = new()
            //{
            //    Type = type,
            //    Value = clientAssertion
            //};

            return this;
        }

        public ClientCredentialRequestBuilder WithClientAssertion(string issuer, JsonWebKey jsonWebKey, string type = OidcConstants.ClientAssertionTypes.JwtBearer)
        {
            //var clientAssertion = ClientAssertionBuilder.CreateClientAssertionJwt(issuer, _request.ClientId, jsonWebKey);
            //_request.ClientAssertion = new()
            //{
            //    Type = type,
            //    Value = clientAssertion
            //};

            return this;
        }

        public ClientCredentialRequestBuilder WithScope(string scope)
        {
            _request.Scope = scope;

            return this;
        }

        public ClientCredentialsTokenRequest Build()
        {
            return _request;
        }
    }
}
