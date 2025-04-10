using Microsoft.AspNetCore.Authentication;

namespace BlazorServerWebApp.Services
{
    public class TokenService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TokenService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string?> GetAccessTokenAsync()
        {
            var authResult = await _httpContextAccessor.HttpContext!.AuthenticateAsync();
            var accessToken = authResult?.Properties?.GetTokenValue("access_token");
            var idToken = authResult?.Properties?.GetTokenValue("id_token");

            return await _httpContextAccessor.HttpContext!.GetTokenAsync("access_token");
        }

        public async Task<string?> GetIdTokenAsync()
        {
            return await _httpContextAccessor.HttpContext!.GetTokenAsync("id_token");
        }

        public async Task<string?> GetRefresAsync()
        {
            return await _httpContextAccessor.HttpContext!.GetTokenAsync("refresh_token");
        }
    }

}
