using Duende.AccessTokenManagement.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Concurrent;
using System.Security.Claims;

/// <summary>
///Blazor Server keeps a persistent SignalR connection. That connection does not have access to HttpContext or cookies directly.
/// https://learn.microsoft.com/en-us/aspnet/core/blazor/state-management?view=aspnetcore-9.0&pivots=server
/// </summary>
public class InMemoryUserTokenStore : IUserTokenStore
{
    public InMemoryUserTokenStore(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    private static readonly ConcurrentDictionary<string, UserToken> _tokenStore = new();
    private readonly IHttpContextAccessor _contextAccessor;

    public Task StoreTokenAsync(ClaimsPrincipal user, UserToken token, UserTokenRequestParameters? parameters = null)
    {
        var userId = user.FindFirst("sub")?.Value;
        if (userId != null)
        {
            _tokenStore[userId] = token;
        }
        return Task.CompletedTask;
    }

    public Task<UserToken> GetTokenAsync(ClaimsPrincipal user, UserTokenRequestParameters? parameters = null)
    {
        var access_token = _contextAccessor.HttpContext?.GetTokenAsync("access_token");
        var userId = user.FindFirst("sub")?.Value;
        if (userId != null && _tokenStore.TryGetValue(userId, out var token))
        {
            return Task.FromResult<UserToken>(token);
        }
        return Task.FromResult<UserToken>(new UserToken());
    }

    public Task ClearTokenAsync(ClaimsPrincipal user, UserTokenRequestParameters? parameters = null)
    {
        var userId = user.FindFirst("sub")?.Value;
        if (userId != null)
        {
            _tokenStore.Remove(userId, out _);
        }
        return Task.CompletedTask;
    }
}