using QuirkLink.Middleware;

namespace QuirkLink.Extensions
{
    public static class RequestLoggingMiddlewareExtensions
    {
        /// <summary>
        /// Adds request logging middleware to the pipeline
        /// </summary>
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}
