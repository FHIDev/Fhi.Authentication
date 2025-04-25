using Fhi.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;

namespace WebApi.Authorization
{
    /// <summary>
    /// Global handling of the authorization result 
    /// - Enrichs with the WWW-Authenticate header if the user is forbidden.
    /// </summary>
    public class DefaultAccessControlMiddleware : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler defaultHandler = new();

        public async Task HandleAsync(
            RequestDelegate next,
            HttpContext context,
            AuthorizationPolicy policy,
            PolicyAuthorizationResult authorizeResult)
        {
            if (authorizeResult.Forbidden && authorizeResult.AuthorizationFailure is not null)
            {
                var scopeRequirement = authorizeResult.AuthorizationFailure.FailureReasons.FirstOrDefault(x => x.Handler.GetType() == typeof(ScopeHandler));
                if (scopeRequirement != null)
                    context.Response.Headers.Append("WWW-Authenticate", $"error=\"insufficient_scope\", error_description=\" {scopeRequirement?.Message}");
            }

            await defaultHandler.HandleAsync(next, context, policy, authorizeResult);
        }
    }
}
