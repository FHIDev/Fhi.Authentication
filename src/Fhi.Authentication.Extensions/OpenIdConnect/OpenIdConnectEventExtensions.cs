using Fhi.Authentication.Tokens;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace Fhi.Authentication.OpenIdConnect
{
    public static class OpenIdConnectEventsExtensions
    {
        private const string ClientAssertionType = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer";

        public static async Task AuthorizationCodeReceivedWithClientAssertionAsync(this AuthorizationCodeReceivedContext context)
        {
            await AuthorizationCodeReceivedWithClientAssertionAsync(context, context.Options.ClientId!, context.Options.ClientSecret!);
        }

        public static async Task AuthorizationCodeReceivedWithClientAssertionAsync(this AuthorizationCodeReceivedContext context, string clientId, string jwk)
        {
            if (context.Options.ConfigurationManager is not null)
            {
                var discovery = await context.Options.ConfigurationManager.GetConfigurationAsync(default);

                context.TokenEndpointRequest!.ClientAssertionType = ClientAssertionType;
                context.TokenEndpointRequest.ClientAssertion = ClientAssertionTokenHandler.CreateJwtToken(discovery.Issuer, clientId, jwk);
            }
        }

#if NET9_0

        public static async Task PushAuthorizationWithClientAssertion(this PushedAuthorizationContext context)
        {
            await PushAuthorizationWithClientAssertion(context, context.Options.ClientId!, context.Options.ClientSecret!);
        }

        public static async Task PushAuthorizationWithClientAssertion(this PushedAuthorizationContext context, string clientId, string jwk)
        {
            if (context.Options.ConfigurationManager is not null)
            {
                var discovery = await context.Options.ConfigurationManager.GetConfigurationAsync(default);

                context.ProtocolMessage.ClientAssertionType = ClientAssertionType;
                context.ProtocolMessage.ClientAssertion = ClientAssertionTokenHandler.CreateJwtToken(discovery.Issuer, clientId, jwk);
            }
        }
#endif
    }
}
