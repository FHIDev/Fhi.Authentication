using Duende.AccessTokenManagement.OpenIdConnect;

namespace Fhi.Authentication.OpenIdConnect
{
    public record TokenResponse(bool IsError = false);

    public interface ITokenService
    {
        Task<TokenResponse> RefreshAccessTokenAsync(string refreshToken);
    }

    public class DefaultTokenService : ITokenService
    {
        private readonly IUserTokenEndpointService _userTokenEndpointService;

        public DefaultTokenService(IUserTokenEndpointService userTokenEndpointService)
        {
            _userTokenEndpointService = userTokenEndpointService;
        }
        public async Task<TokenResponse> RefreshAccessTokenAsync(string refreshToken)
        {
            var refreshedTokens = await _userTokenEndpointService.RefreshAccessTokenAsync(new UserToken() { RefreshToken = refreshToken }, new UserTokenRequestParameters());
            return new TokenResponse();
        }
    }
}
