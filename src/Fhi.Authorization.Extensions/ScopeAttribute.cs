using Microsoft.AspNetCore.Authorization;

namespace Fhi.Authorization
{
    /// <summary>
    /// This attribute is used to specify the scope required for a specific action or controller.
    /// </summary>
    /// <param name="permission"></param>
    public class ScopeAttribute(string permission) : AuthorizeAttribute, IAuthorizationRequirement, IAuthorizationRequirementData
    {
        /// <summary>
        /// Scope required for the action or controller.
        /// </summary>
        public string Scope { get; private set; } = permission;

        /// <inheritdoc/>
        public IEnumerable<IAuthorizationRequirement> GetRequirements()
        {
            yield return this;
        }
    }
}
