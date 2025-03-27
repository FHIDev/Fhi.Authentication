using Fhi.Authentication.Tokens;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace Fhi.Authentication.OpenIdConnect
{
    public class DefaultOpenIdConnectEvents : OpenIdConnectEvents
    {
        private readonly IClientAssertionTokenHandler _clientAssertionTokenHandler;

        public DefaultOpenIdConnectEvents(IClientAssertionTokenHandler clientAssertionTokenHandler)
        {
            _clientAssertionTokenHandler = clientAssertionTokenHandler;
        }

        public override async Task AuthorizationCodeReceived(AuthorizationCodeReceivedContext context)
        {
            if (context.Options.ConfigurationManager is not null)
            {
                var discovery = await context.Options.ConfigurationManager.GetConfigurationAsync(default);

                context.TokenEndpointRequest!.ClientAssertionType = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer";
                context.TokenEndpointRequest.ClientAssertion = _clientAssertionTokenHandler.CreateJwtToken(discovery.Issuer, context.Options.ClientId, context.Options.ClientSecret);
            }
        }

#if NET9_0

        public override async Task PushAuthorization(PushedAuthorizationContext context)
        {
            if (context.Options.ConfigurationManager is not null)
            {
                var discovery = await context.Options.ConfigurationManager.GetConfigurationAsync(default);

                context.ProtocolMessage.ClientAssertionType = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer";
                context.ProtocolMessage.ClientAssertion = _clientAssertionTokenHandler.CreateJwtToken(discovery.Issuer, context.Options.ClientId, context.Options.ClientSecret);
            }
        }
#endif


        public override Task AuthenticationFailed(AuthenticationFailedContext context)
        {
            return base.AuthenticationFailed(context);
        }
    }
}
