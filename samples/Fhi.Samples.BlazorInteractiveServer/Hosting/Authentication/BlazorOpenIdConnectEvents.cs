using Duende.AccessTokenManagement.OpenIdConnect;
using Fhi.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;

namespace BlazorInteractiveServer.Hosting.Authentication
{
    public class BlazorOpenIdConnectEvents(IUserTokenStore UserTokenStore, IOptions<AuthenticationSettings> Options) : OpenIdConnectEvents
    {
        public override Task RedirectToIdentityProvider(RedirectContext context)
        {
            //if (context.Request.Path.StartsWithSegments("/_blazor"))
            //{
            //context.Response.Headers.Location = context.ProtocolMessage.CreateAuthenticationRequestUrl();
            //context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //context.HandleResponse();
            //}
            return base.RedirectToIdentityProvider(context);
        }

        public override async Task PushAuthorization(PushedAuthorizationContext context)
        {
            await context.PushAuthorizationWithClientAssertion(Options.Value.ClientId, Options.Value.ClientSecret);
        }

        public override async Task AuthorizationCodeReceived(AuthorizationCodeReceivedContext context)
        {
            await context.AuthorizationCodeReceivedWithClientAssertionAsync(Options.Value.ClientId, Options.Value.ClientSecret);
        }

        public override async Task<Task> TokenValidated(TokenValidatedContext context)
        {
            var exp = DateTimeOffset.UtcNow.AddSeconds(double.Parse(context.TokenEndpointResponse!.ExpiresIn));
            await UserTokenStore.StoreTokenAsync(context.Principal!, new UserToken
            {
                AccessToken = context.TokenEndpointResponse.AccessToken,
                AccessTokenType = context.TokenEndpointResponse.TokenType,
                Expiration = exp,
                RefreshToken = context.TokenEndpointResponse.RefreshToken,
                Scope = context.TokenEndpointResponse.Scope
            });

            return base.TokenValidated(context);
        }
    }
}
