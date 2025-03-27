namespace ClientCredential.WorkerService.Utility
{
    public class HttpRequestMessageBuilder
    {
        private static HttpRequestMessage? _httpRequest;
        public HttpRequestMessageBuilder Create(HttpMethod method, Uri uri)
        {
            _httpRequest = new HttpRequestMessage(method, uri);
            return new();
        }

        public HttpRequestMessageBuilder WithDpop(string uri, string httpMethod, string privateJwk, string privateJwkAlg, string accessToken)
        {
            //var dpopProof = DPoPProofBuilder.CreateDPoPProof(
            //    uri,
            //    httpMethod,
            //    privateJwk,
            //    privateJwkAlg,
            //    accessToken: accessToken);

            //_httpRequest.SetDPoPToken(accessToken, dpopProof);
            return this;
        }

        public HttpRequestMessageBuilder WithHeader(string key, string value)
        {
            _httpRequest.Headers.Add(key, value);
            return this;
        }

        public HttpRequestMessageBuilder WithContent(HttpContent content)
        {
            _httpRequest.Content = content;
            return this;
        }

        public HttpRequestMessage Build()
        {
            return _httpRequest;
        }
    }
}
