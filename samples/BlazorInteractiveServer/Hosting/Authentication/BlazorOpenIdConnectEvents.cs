using Duende.AccessTokenManagement.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace BlazorInteractiveServer.Hosting.Authentication
{
    public class BlazorOpenIdConnectEvents : OpenIdConnectEvents
    {
        private readonly IUserTokenStore _userTokenStore;

        public BlazorOpenIdConnectEvents(IUserTokenStore userTokenStore)
        {
            _userTokenStore = userTokenStore;
        }

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

        public override Task PushAuthorization(PushedAuthorizationContext context)
        {
            //await context.PushAuthorizationWithClientAssertion();
            return base.PushAuthorization(context);
        }

        public override Task AuthorizationCodeReceived(AuthorizationCodeReceivedContext context)
        {
            //await context.AuthorizationCodeReceivedWithClientAssertionAsync();
            return base.AuthorizationCodeReceived(context);
        }

        public override async Task<Task> TokenValidated(TokenValidatedContext context)
        {
            var exp = DateTimeOffset.UtcNow.AddSeconds(double.Parse(context.TokenEndpointResponse!.ExpiresIn));
            await _userTokenStore.StoreTokenAsync(context.Principal!, new UserToken
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
