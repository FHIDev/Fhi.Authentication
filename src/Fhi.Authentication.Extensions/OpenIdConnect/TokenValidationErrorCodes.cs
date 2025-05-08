namespace Fhi.Authentication.OpenIdConnect
{
    /// <summary>
    /// Errorcodes for token validation
    /// </summary>
    public static class TokenValidationErrorCodes
    {
        /// <summary>
        /// Token not found
        /// </summary>
        public const string NotFound = "NotFound";

        /// <summary>
        /// Refreshtoken is expired
        /// </summary>
        public const string ExpiredRefreshToken = "ExpiredRefreshToken";
    }
}
