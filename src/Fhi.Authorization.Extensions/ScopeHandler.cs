using Microsoft.AspNetCore.Authorization;

namespace Fhi.Authorization
{
    /// <summary>
    /// Handler for validate if the user has the required scope in their claims.
    /// </summary>
    public class ScopeHandler : AuthorizationHandler<ScopeAttribute>
    {
        /// <summary>
        /// Handles the requirement for the specified scope. Checks if the user has the required scope in their claims.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement"></param>
        /// <returns></returns>
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
