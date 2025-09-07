
namespace QuirkLink.Models.ServiceBus
{
    public class CleanupQueueServiceBusMessage : QuirkLinkMessageBase
    {
        public CleanupQueueServiceBusMessage(
            string token, DateTime linkGenerationTime, uint expirationTimeInSeconds) 
            : base(token, linkGenerationTime)
        {
            ExpirationTimeInSeconds = expirationTimeInSeconds;
        }
        public uint ExpirationTimeInSeconds { get; }
    }
}
