namespace WorkerService.ClientCredential
{
    public class ClientConfiguration
    {
        public string Authority { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string PrivateJwk { get; set; } = string.Empty;
        public string Scope { get; set; } = string.Empty;

        /// <summary>
        /// Must be set for 
        /// </summary>
        public string TokenEndpoint { get; set; } = string.Empty;
    }
}
