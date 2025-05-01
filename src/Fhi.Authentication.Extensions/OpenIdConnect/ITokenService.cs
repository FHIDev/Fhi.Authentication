using Duende.AccessTokenManagement.OpenIdConnect;
using Microsoft.Extensions.Logging;

namespace Fhi.Authentication.OpenIdConnect
{
    public record TokenResponse(bool IsError = false);
    /// <summary>
    /// Abstraction for token service.
    /// TODO: response should be improved
    /// </summary>
    public interface ITokenService
    {
        Task<TokenResponse> RefreshAccessTokenAsync(string refreshToken);
    }

    internal class DefaultTokenService(IUserTokenEndpointService UserTokenEndpointService, ILogger<DefaultTokenService> Logger) : ITokenService
    {
        public async Task<TokenResponse> RefreshAccessTokenAsync(string refreshToken)
        {
            var userToken = await UserTokenEndpointService.RefreshAccessTokenAsync(new UserToken() { RefreshToken = refreshToken }, new UserTokenRequestParameters());
            if (userToken.IsError)
            {
                Logger.LogError(userToken.Error);
                return new TokenResponse(true);
            }

            return new TokenResponse();
        }
    }
}
