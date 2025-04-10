using Duende.AccessTokenManagement.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using System.Net;

public class RefreshTokenFailureHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserTokenManagementService _userTokenManagementService;

    public RefreshTokenFailureHandler(IHttpContextAccessor httpContextAccessor, IUserTokenManagementService userTokenManagementService)
    {
        _httpContextAccessor = httpContextAccessor;
        _userTokenManagementService = userTokenManagementService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            var context = _httpContextAccessor.HttpContext;

            var userToken = await context.GetUserAccessTokenAsync();

            if (userToken.IsError)
            {
                //await context.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties { RedirectUri = "/" });
                // context.Response.Redirect("/login");
            }
        }

        return response;
    }
}
