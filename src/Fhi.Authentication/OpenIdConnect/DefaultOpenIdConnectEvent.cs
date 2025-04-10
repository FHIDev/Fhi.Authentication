using Fhi.Authentication.Tokens;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;

namespace Fhi.Authentication.OpenIdConnect
{

    public static class OpenIdConnectEventsExtensions
    {
        public static async Task AuthorizationCodeReceivedWithClientAssertion(this AuthorizationCodeReceivedContext context)
        {
            if (context.Options.ConfigurationManager is not null)
            {
                var discovery = await context.Options.ConfigurationManager.GetConfigurationAsync(default);
                var clientAssertionTokenHandler = context.HttpContext.RequestServices.GetRequiredService<IClientAssertionTokenHandler>();

                context.TokenEndpointRequest!.ClientAssertionType = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer";
                context.TokenEndpointRequest.ClientAssertion = clientAssertionTokenHandler.CreateJwtToken(discovery.Issuer, context.Options.ClientId, context.Options.ClientSecret);
            }
        }

#if NET9_0

        public static async Task PushAuthorizationWithClientAssertion(this PushedAuthorizationContext context)
        {
            if (context.Options.ConfigurationManager is not null)
            {
                var discovery = await context.Options.ConfigurationManager.GetConfigurationAsync(default);
                 var clientAssertionTokenHandler = context.HttpContext.RequestServices.GetRequiredService<IClientAssertionTokenHandler>();

                context.ProtocolMessage.ClientAssertionType = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer";
                context.ProtocolMessage.ClientAssertion = clientAssertionTokenHandler.CreateJwtToken(discovery.Issuer, context.Options.ClientId, context.Options.ClientSecret);
            }
        }
#endif
    }

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
