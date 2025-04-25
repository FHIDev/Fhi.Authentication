using Microsoft.AspNetCore.Authorization;

namespace Fhi.Authorization
{
    /// <summary>
    /// 
    /// </summary>
    public class ScopeHandler : AuthorizationHandler<ScopeAttribute>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ScopeAttribute requirement)
        {
            if (context.User.HasClaim(c => c.Type == "scope" && c.Value == requirement.Scope))
            {
                context.Succeed(requirement);

                return Task.CompletedTask;
            }

            context.Fail(new AuthorizationFailureReason(this, $"Access token is missing required scope.\", scope=\"{requirement.Scope}\""));
            return Task.CompletedTask;
        }
    }
}
