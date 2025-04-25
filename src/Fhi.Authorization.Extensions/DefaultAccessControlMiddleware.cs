namespace Fhi.Authorization
{
    //public class DefaultAccessControlMiddleware : IAuthorizationMiddlewareResultHandler
    //{
    //    private readonly AuthorizationMiddlewareResultHandler defaultHandler = new();

    //    public async Task HandleAsync(
    //        RequestDelegate next,
    //        HttpContext context,
    //        AuthorizationPolicy policy,
    //        PolicyAuthorizationResult authorizeResult)
    //    {
    //        if (authorizeResult.Forbidden && authorizeResult.AuthorizationFailure is not null)
    //        {
    //            var scopeRequirement = authorizeResult.AuthorizationFailure.FailureReasons.FirstOrDefault(x => x.Handler.GetType() == typeof(ScopeHandler));
    //            if (scopeRequirement != null)
    //                context.Response.Headers.Append("WWW-Authenticate", $"error=\"insufficient_scope\", error_description=\" {scopeRequirement?.Message}");
    //        }

    //        await defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    //    }
    //}
}
