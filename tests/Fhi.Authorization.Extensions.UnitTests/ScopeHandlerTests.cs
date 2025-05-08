using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Fhi.Authorization.IntegrationTests
{
    public class ScopeHandlerTests
    {
        [Test]
        public async Task HandleRequirementAsync_ScopeExist_Ok()
        {
            var scopeHandler = new ScopeHandler();
            var context = new AuthorizationHandlerContext(
                [new ScopeAttribute("test")],
                new ClaimsPrincipal(new ClaimsIdentity(
                [
                    new Claim("scope", "test")
                ])),
                null);

            await scopeHandler.HandleAsync(context);

            Assert.That(context.HasSucceeded, Is.True);
            Assert.That(context.HasFailed, Is.False);
        }

        [Test]
        public async Task HandleRequirementAsync_NoScopeClaim_Fail()
        {
            var scopeHandler = new ScopeHandler();
            var context = new AuthorizationHandlerContext(
                [new ScopeAttribute("test")],
                new ClaimsPrincipal(new ClaimsIdentity(
                [
                ])),
                null);

            await scopeHandler.HandleAsync(context);

            Assert.That(context.HasSucceeded, Is.False);
            Assert.That(context.HasFailed, Is.True);
        }

        [Test]
        public async Task HandleRequirementAsync_ScopeDoesNotExist_Fail()
        {
            var scopeHandler = new ScopeHandler();
            var context = new AuthorizationHandlerContext(
                [new ScopeAttribute("fail")],
                new ClaimsPrincipal(new ClaimsIdentity(
                [
                    new Claim("scope", "test")
                ])),
                null);

            await scopeHandler.HandleAsync(context);

            Assert.That(context.HasSucceeded, Is.False);
            Assert.That(context.HasFailed, Is.True);
        }
    }
}
