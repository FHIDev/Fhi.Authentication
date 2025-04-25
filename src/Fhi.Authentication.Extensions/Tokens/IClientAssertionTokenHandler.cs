using Microsoft.IdentityModel.Tokens;

namespace Fhi.Authentication.Tokens
{

    public interface IClientAssertionTokenHandler
    {
        public string CreateJwtToken(string clientId, string audience, string jwk);

        public string CreateJwtToken(string issuer, string clientId, JsonWebKey jwk);

        //public string CreateJwtToken(string issuer, string clientId, string certificatePath, string certificatePassword);
    }
}
