
/// <summary>
/// Note: This is not production code. It is sample code for demonstration of requests only.
/// </summary>
public class LoggingHandler : DelegatingHandler
{
    private readonly ILogger<LoggingHandler> _logger;

    public LoggingHandler(ILogger<LoggingHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Log request details
        _logger.LogInformation("Request:");
        _logger.LogInformation($"Method: {request.Method}");
        _logger.LogInformation($"URL: {request.RequestUri}");

        if (request.Headers.Authorization != null)
        {
            _logger.LogInformation($"Authorization: {request.Headers.Authorization.Scheme} {request.Headers.Authorization.Parameter}");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
