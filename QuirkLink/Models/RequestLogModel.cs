namespace QuirkLink.Models
{
    public class RequestLogModel
    {
        public string RequestId { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string QueryString { get; set; } = string.Empty;
        public string ClientIpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public string Referer { get; set; } = string.Empty;
        public Dictionary<string, string> Headers { get; set; } = new();
        public string RequestBody { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public long DurationMs { get; set; }
        public DateTime RequestTimestamp { get; set; }
        public DateTime ResponseTimestamp { get; set; }
        public string Environment { get; set; } = string.Empty;
        public string ServerName { get; set; } = string.Empty;
    }
}
