using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Fhi.Authentication.Tokens
{
    public class DefaultClientAssertionTokenHandler : IClientAssertionTokenHandler
    {
        public string CreateJwtToken(string issuer, string clientId, string jwk)
        {
            var securityKey = new JsonWebKey(jwk);
            string token = CreateJwtToken(issuer, clientId, securityKey);

            return token;
        }

        public string CreateJwtToken(string issuer, string clientId, JsonWebKey securityKey)
        {
            //Create payload
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, clientId),
                new(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            };
            var payload = new JwtPayload(clientId, issuer, claims, DateTime.UtcNow, DateTime.UtcNow.AddSeconds(60));

            //Create header
            if (string.IsNullOrEmpty(securityKey.Alg))
                securityKey.Alg = SecurityAlgorithms.RsaSha256;
            var signingCredentials = new SigningCredentials(securityKey, securityKey.Alg);
            var header = new JwtHeader(signingCredentials, null, "client-authentication+jwt");

            //create signed JWT with header and payload 
            var jwtSecurityToken = new JwtSecurityToken(header, payload);
            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return token;
        }
    }
}
