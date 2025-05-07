namespace WorkerService
{
    public class ClientConfiguration
    {
        /// <summary>
        /// Used for discovery document and manual processes
        /// </summary>
        public string Authority { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string Secret { get; set; } = string.Empty;
        public string Scope { get; set; } = string.Empty;

        /// <summary>
        /// Must be set for duende HttpClient extensions
        /// </summary>
        public string TokenEndpoint { get; set; } = string.Empty;
    }
}
