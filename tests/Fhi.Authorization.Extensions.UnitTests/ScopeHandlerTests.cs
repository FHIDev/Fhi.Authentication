using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Fhi.Authorization.IntegrationTests
{
    public class ScopeHandlerTests
    {

        [Test]
        public void HandleRequirementAsync_ScopeExist_Ok()
        {
            var scopeHandler = new ScopeHandler();
            var context = new AuthorizationHandlerContext(
                [new ScopeAttribute("test")],
                new ClaimsPrincipal(new ClaimsIdentity(
                [
                    new Claim("scope", "test")
                ])),
                null);
            scopeHandler.HandleAsync(context);

            Assert.Pass();
        }

    }
}
