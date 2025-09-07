using System.Diagnostics;
using System.Text;

namespace QuirkLink.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestId = Guid.NewGuid().ToString("N")[..8];

            // Log request
            LogRequest(context, requestId);

            // Continue to next middleware
            await _next(context);

            stopwatch.Stop();

            // Log response
            LogResponse(context, requestId, stopwatch.ElapsedMilliseconds);
        }

        private void LogRequest(HttpContext context, string requestId)
        {
            var request = context.Request;
            var clientIp = GetClientIp(context);
            var userAgent = request.Headers.UserAgent.ToString();

            var log = new StringBuilder();
            log.AppendLine($"[{requestId}] REQUEST");
            log.AppendLine($"  Method: {request.Method}");
            log.AppendLine($"  Path: {request.Path}{request.QueryString}");
            log.AppendLine($"  IP: {clientIp}");
            log.AppendLine($"  User-Agent: {userAgent}");
            log.AppendLine($"  Time: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");

            _logger.LogInformation(log.ToString());
        }

        private void LogResponse(HttpContext context, string requestId, long elapsedMs)
        {
            var log = new StringBuilder();
            log.AppendLine($"[{requestId}] RESPONSE");
            log.AppendLine($"  Status: {context.Response.StatusCode}");
            log.AppendLine($"  Duration: {elapsedMs}ms");

            if (context.Response.StatusCode >= 400)
            {
                _logger.LogWarning(log.ToString());
            }
            else
            {
                _logger.LogInformation(log.ToString());
            }
        }

        private static string GetClientIp(HttpContext context)
        {
            // Check for forwarded IP first (for load balancers/proxies)
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                return forwardedFor.Split(',')[0].Trim();
            }

            // Fall back to connection IP
            return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }
    }
}
