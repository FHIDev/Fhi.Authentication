using Fhi.Authentication.Tokens;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;

namespace Fhi.Authentication.OpenIdConnect
{
    public static class OpenIdConnectEventsExtensions
    {

        public static async Task AuthorizationCodeReceivedWithClientAssertionAsync(this AuthorizationCodeReceivedContext context)
        {
            if (context.Options.ConfigurationManager is not null)
            {
                var discovery = await context.Options.ConfigurationManager.GetConfigurationAsync(default);
                var clientAssertionTokenHandler = context.HttpContext.RequestServices.GetRequiredService<IClientAssertionTokenHandler>();

                context.TokenEndpointRequest!.ClientAssertionType = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer";
                context.TokenEndpointRequest.ClientAssertion = clientAssertionTokenHandler.CreateJwtToken(discovery.Issuer, context.Options.ClientId!, context.Options.ClientSecret!);
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
                context.ProtocolMessage.ClientAssertion = clientAssertionTokenHandler.CreateJwtToken(discovery.Issuer, context.Options.ClientId!, context.Options.ClientSecret!);
            }
        }
#endif
    }

}
