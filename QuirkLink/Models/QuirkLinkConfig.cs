namespace QuirkLink.Models
{
    public class QuirkLinkConfig
    {
        public required string Aes256Key { get; set; }
        
        /// <summary>
        /// The Redis connection string for storage
        /// </summary>
        public required string RedisConnectionString { get; set; }
        
        /// <summary>
        /// The prefix for Redis keys
        /// </summary>
        public string RedisKeyPrefix { get; set; } = "ql:link:";

        /// <summary>
        /// Service Bus connection string.
        /// </summary>
        public required string ServiceBusConnectionString { get; set; }

        /// <summary>
        /// Cleanup queue name.
        /// </summary>
        public required string CleanupQueueName { get; set; }

        /// <summary>
        /// Tracking queue name.
        /// </summary>
        public required string TrackingQueueName { get; set; }
        
        /// <summary>
        /// Base URL for QuirkLinks in development environment (e.g., http://localhost:5198)
        /// </summary>
        public string? DevelopmentBaseUrl { get; set; }
        
        /// <summary>
        /// Base URL for QuirkLinks in production environment (e.g., https://quirklink.com)
        /// </summary>
        public string? ProductionBaseUrl { get; set; }
        
        /// <summary>
        /// Base URL for QuirkLinks in test environment
        /// </summary>
        public string? TestBaseUrl { get; set; }
        
        /// <summary>
        /// Default scheme for QuirkLinks when no environment-specific URL is configured
        /// </summary>
        public string DefaultScheme { get; set; } = "quirklink://";
    }
}
