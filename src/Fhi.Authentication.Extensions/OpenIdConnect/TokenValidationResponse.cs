namespace Fhi.Authentication.OpenIdConnect
{
public static partial class CookieEventExtensions
    {
        /// <summary>
        /// Response for token validation.
        /// </summary>
        /// <param name="IsError">Values True or False</param>
        /// <param name="Error">Error type</param>
        /// <param name="ErrorDescription">Description of error</param>
        public record TokenValidationResponse(bool IsError, string Error = "", string ErrorDescription = "");
    }
}
