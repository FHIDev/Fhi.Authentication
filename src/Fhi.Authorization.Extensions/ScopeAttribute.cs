using Microsoft.AspNetCore.Authorization;

namespace Fhi.Authorization
{
    public class ScopeAttribute(string permission) : AuthorizeAttribute, IAuthorizationRequirement, IAuthorizationRequirementData
    {
        public string Scope { get; private set; } = permission;

        public IEnumerable<IAuthorizationRequirement> GetRequirements()
        {
            yield return this;
        }
    }
}
