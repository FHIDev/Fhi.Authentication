namespace WorkerService.ClientCredential
{
    public class ClientConfiguration
    {
        public string ClientName { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string PrivateJwk { get; set; } = string.Empty;
        public string Scope { get; set; } = string.Empty;
        public string TokenEndpoint { get; set; } = string.Empty;
    }
}
