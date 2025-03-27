using Fhi.Authentication.Tokens;

namespace Fhi.Authentication.UnitTests
{
    public class DefaultClientAssertionTokenServiceTests
    {
        [Test]
        public void GenerateClientAssertion_algorithmIsEmpty_ThrowException()
        {
            var service = new DefaultClientAssertionTokenHandler();
        }
    }
}