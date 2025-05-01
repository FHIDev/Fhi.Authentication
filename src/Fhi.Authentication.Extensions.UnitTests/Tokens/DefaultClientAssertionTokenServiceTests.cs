using Fhi.Authentication.Tokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Fhi.Authentication.UnitTests.Tokens
{
    public class DefaultClientAssertionTokenServiceTests
    {
        //[Test]
        //public void GenerateClientAssertion_keyFormatXX_ThrowException()
        //{
        //    var privateKeyString = "\"{\\\\u0022alg\\\\u0022:\\\\u0022PS512\\\\u0022,\\\\u0022d\\\\u0022:\\\\u0022Fddoqu2cGrlbcRjHkZAl8yfbw8xVyiVGCspJW5YlaKluQjjjEOqvujdZK9KePDuWdaZIx6YqKsulF1XuL18NZ8qfr_JY9Swzwc8Cc8eoxhBnKv2ByCSS3vrDmWi9ic6HucO2_CMY-t4yqj8mDQERqWOy96pOaZR-wqDFw-bWZ9XExtLDJI1btXinDkmBkUElVhjZYUZlfm2mZ5LsWw168GoGD4Pt0iFgqVUfH0ET_ozpffM9RZ9J7pk8ukHt9KG25FhMmDVfvXfK3T1l0tqGvpG4g6EiscRp8rm12aIMtwDw4S6DjJbvVHouPYBzWBkegORRV0aIGD4DHbDyrLrBlvvXdDc3Lo5Z7Ik6lcHdiONVwkeZ2PNVkZF8uK6thUxw-1-UiLTAPJE0_8u8aa6Dx5OYko_zpQiuu56ATk0al7-NubTocqjHfOAabiQ6nrvcnsf2qjFAT8kaexUQQNzOrMu_V7jbiQcYeyjGcgI7N4oPOBFhaIEuWYdxDkSNxLjVd5UAwpN4qez87tNY_xdkpBVDsH6FirdtSiGEP7stRkpOHAzws4725v9TXMK6oSXA4FRwMHh9JrTTi-9boC3nAa7or_wofHdRZN7UT84GAQfZQzYkxlHZgvLAyjkHYbD3qnVKRtFXIQMaAID6mZsQgRzyy4M76M_2FB8oIY7T9z0\\\\u0022,\\\\u0022dp\\\\u0022:\\\\u0022SKEK4z81S6hKe8ebXfeqSAq5XyOiYu34-X7RoPJu2GVTR9ozC3S4LVQEx7UXGaYT3PlexA9Vi2XdWOmIgrlcTerpq5niU-juRQ_lE-rF3-i7vSiVRb1hVAkbPg0t4TmUycKMEudRVnCw243n98hsz1ckNTr-91geqyMJLyqDmsuljJuvYlE_hu4MQ33trbaIO3Z0IswPPOPBi2P0cUoXP3r-sawiT2v-lXnfeOJJc-3T-Hi6GgjQ3qVgM8i-eORk2dLNqdFKwbu2hcmvQg_FRr4eoFA1S6cgQNNqinLa1g_GkXM6oshN6RugEenciqninYdTJHa0aoPcn0TvFSdwiQ\\\\u0022,\\\\u0022dq\\\\u0022:\\\\u0022M27qHq-o4i8ufqfnyv2dn_8UOMusI7seNH_2_Y3oWRy44HiryWRZsmsUX_hxrkiU-ciwh-TXS_T8BSJc9Dqpn4VNf9JeaeaE4gioXK1ADflpy39bkb7mw_8H7YfxIXgBOZIi53WCtiCTKGam_mSZLNvt3BBvtbSnv8Mhs0zRJyVRPsSMoCxXBJ4g31T-3wOm2pQnLf5z2Xm6xizwzdtdKLLvegHi8uR8Z5SVtnDYZF_W86A-_7Vwl1Ev8ZXB9TeameszrncSaUqK4YT808cijeP2vVuTmxjn3yIQDuW18lAl-wL6yGXEkM-2O0O_BMpU17sbN3cJywv9EN3V8KZPPQ\\\\u0022,\\\\u0022e\\\\u0022:\\\\u0022AQAB\\\\u0022,\\\\u0022kty\\\\u0022:\\\\u0022RSA\\\\u0022,\\\\u0022n\\\\u0022:\\\\u0022qPMB9YYNNtb803Q-X_xcA9ydiGFXcVrg5SneL2SaH3rHI6S6XNguMkfdKrP-sjw9ftoEYRSWFCXi_hy4aemDU5d_1-94IxW85FSr45nUBzoQ30B1Ki0kaUJqdD-5AqVa0kCIU9Hsz-rlQfg-XoR0SGB1VmEGJTj7Pqrwvxm1HchU3NxUZq_kSY4iUqf2iQxkHGh4FGs6PCIMNrrhUMaixGYkdMCDMnNbKo1FT3UbeD1HMsyQow8rdiHZ4hOOSUMrZ_35Rv15t80glpn6x8O9A-1Ml23T4fBeusBHKur-5hRzygRXx2QWCht8y41EeDX7nn1PcGmdMFUwI6f2n5xb-WQ4lGzBkTnEmQiZN1n4fVpSuk9hnpgfA3VeXNrdoPpMaSIZ6JqVRRQFPwuNQhYF65N9en8SkSjqCaUo3vsmrChb4ZbKaiR_cJJ-jW9wPwwDMkE1QLX-87XQL-UrUwhJYc6665hcsJ9NMx0P36cS5gawoUYpCMfsSEK5zWbsESM-PB1bhYvkBbi_gLNd8f99iMqCX_Awb3b6l3rA21K0Cg3NDJd735P8uX8JMyzfVHF6ZPH3sJq6s4GoDix3MoJSMUHVNsXKcUiwkrs5cQ3YDxy0ZF1cVDPnyqP6JK8j92gg2gGN7YBWIvKBUipuby9iLG5g_s8E7v8fj8TY_7Efd58\\\\u0022,\\\\u0022p\\\\u0022:\\\\u002229YLfunTY4Y-kfj9jzGE8E94Sh7ysYtTX0tH7kQBc8bReiEjxgPryFZ6QCb0rpMNtDP4-Fz_gxc63bLtpzRv41SyqE__By4K2pGwf0_-nOdPAKp5hkw552-my14EzhlTawR3n19Ah3kEEfT5loo-_gNaPZEZVGh-yQSLQhzhv2NeKhsZX34_kIw3Qc-uCTmILoV1KTHrM7otG6MdDY5QCg2BHomr3AlvZMm33NWa9bqSZpaEvQUACnp6B_TditcostwxVaLiwtTS8m3gEufrdP9Cbc1F5fI8iQFpTCi8JQOyQ45y_pGT51S3eEpTw1nfdIHIDJoXwXswg04oIS6--w\\\\u0022,\\\\u0022q\\\\u0022:\\\\u0022xL33B2SY-Rzc7cqxpJs51Zr3SXfGBDPY90qY8DhFE6okvr3qbL_XOFPlVFRbXaUctXrIUfZGkVRRIuzNCrzN0BSD-9_CQ5TRfnn95e98M1zFXdZq6Th5IfW_qyC_ZbJfQ2xhgUTPY3UtyB1G8edW4RJf1mBP8QD2jEm3Jk3LX71stCqqQlOh_-1vPAy0I9arLZpU31EHGKmvKas0SzC1_luYvEnjHSxSevygpBm-EId7ueFohKGOe4lZnkZ9Old3ogMpZitQllR79Ynneg9dgcdCjl8TLjkXtqJYzJx8BLF0ZmV7g_2BD-Mte5089MTW5UxK7WavTMiFto8Yjoy4rQ\\\\u0022,\\\\u0022qi\\\\u0022:\\\\u00220VsLnmpVm6BHtPkrxv-QkXc5OEY21qNz5cDAAtxxVZHzRsx3jwfUCfZRWoUXLQt7oLiN_abAPUyDF5BrB7zZHMx0BHqDxrFfF_9pK5Kqm1MOfdSlKbxLBU4crC94Xc-n0qU3mm8Z-o51mdr7Vlvpxe5lC_iIC6KEhKM-_92UmQIV96AYEZqIDzqp0Ceom4Iz-AOQRdE4VzHh-jYwj32MohRXVCpK6zjgyVZn7rJKfNF-klPTeLzkOLSYG8rvQ_E4p56uf1tsV-m0NCf41hUPA54TVSzm56ToEy13WDSGNi6HUFGj7FqQF2HQgyWhPhuYK1mva8RKe5cmuOJiDd_J7w\\\\u0022}\"";

        //    var token = ClientAssertionTokenHandler.CreateJwtToken("issuer", "clientId", privateKeyString);
        //}

        //[Test]
        //public void GenerateClientAssertion_keyIsEmpty_ThrowException()
        //{
        //    Assert.Throws<ArgumentException>(() => ClientAssertionTokenHandler.CreateJwtToken("issuer", "clientId", ""));
        //}

        /// <summary>
        /// Generate private and public key using JwkGenerator and create a client assertion token from private key using DefaultClientAssertionTokenHandler.
        /// </summary>
        [Test]
        public void GenerateKeysAndClientAssertion()
        {
            var keys = JwkGenerator.GenerateRsaJwk();

            var assertion = ClientAssertionTokenHandler.CreateJwtToken("http://issuer", "clientId", keys.PrivateKey);

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(assertion);

            Assert.Multiple(() =>
            {
                Assert.That(token.Issuer, Is.EqualTo("clientId"), "Issuer mismatch");
                Assert.That(token.Claims.Count, Is.EqualTo(7), "Unexpected claim count");

                var audClaim = token.Claims.SingleOrDefault(x => x.Type == "aud");
                Assert.That(audClaim, Is.Not.Null, "Missing 'aud' claim");
                Assert.That(audClaim!.Value, Is.EqualTo("http://issuer"), "Invalid 'aud' claim value");

                var subClaim = token.Claims.SingleOrDefault(x => x.Type == "sub");
                Assert.That(subClaim, Is.Not.Null, "Missing 'sub' claim");
                Assert.That(subClaim!.Value, Is.EqualTo("clientId"), "Invalid 'sub' claim value");

                Assert.That(token.Claims.Any(x => x.Type == "jti"), Is.True, "Missing 'jit' claim");
                Assert.That(token.Claims.Any(x => x.Type == "nbf"), Is.True, "Missing 'nbf' claim");
                Assert.That(token.Claims.Any(x => x.Type == "iat"), Is.True, "Missing 'iat' claim");
                Assert.That(token.Claims.Any(x => x.Type == "exp"), Is.True, "Missing 'exp' claim");

                var typ = token.Header.SingleOrDefault(x => x.Key == "typ");
                Assert.That(typ.Value, Is.EqualTo("client-authentication+jwt"));

                var alg = token.Header.SingleOrDefault(x => x.Key == "alg");
                Assert.That(alg.Value, Is.EqualTo(SecurityAlgorithms.RsaSha512));

                var jwk = new JsonWebKey(keys.PrivateKey);
                var kid = token.Header.SingleOrDefault(x => x.Key == "kid");
                Assert.That(kid.Value, Is.EqualTo(jwk.Kid));
            });
        }
    }
}