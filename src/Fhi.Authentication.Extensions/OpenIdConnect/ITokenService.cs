using Duende.AccessTokenManagement.OpenIdConnect;
using Microsoft.Extensions.Logging;

namespace Fhi.Authentication.OpenIdConnect
{
    /// <summary>
    /// Response for token validation.
    /// </summary>
    /// <param name="IsError"></param>
    public record TokenResponse(bool IsError = false);
    /// <summary>
    /// Abstraction for token service.
    /// TODO: response should be improved
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Refresh access token.
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        Task<TokenResponse> RefreshAccessTokenAsync(string refreshToken);
    }

    internal class DefaultTokenService(IUserTokenEndpointService UserTokenEndpointService, ILogger<DefaultTokenService> Logger) : ITokenService
    {
        public async Task<TokenResponse> RefreshAccessTokenAsync(string refreshToken)
        {
            var userToken = await UserTokenEndpointService.RefreshAccessTokenAsync(new UserToken() { RefreshToken = refreshToken }, new UserTokenRequestParameters());
            if (userToken.IsError)
            {
                Logger.LogError(message: userToken.Error);
                return new TokenResponse(true);
            }

            return new TokenResponse();
        }
    }
}
