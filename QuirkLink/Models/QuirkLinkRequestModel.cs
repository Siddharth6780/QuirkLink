using System.ComponentModel.DataAnnotations;

namespace QuirkLink.Models
{
    public class QuirkLinkRequestModel
    {
        [StringLength(100)]
        public required string Content { get; set; }

        /// <summary>
        /// Expiration time in seconds. Default is 86400 seconds (24 hours).
        /// </summary>
        public required uint ExpireSeconds { get; set; } = 86400;
    }
}