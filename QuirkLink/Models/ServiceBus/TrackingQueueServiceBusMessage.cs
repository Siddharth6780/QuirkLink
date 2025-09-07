
namespace QuirkLink.Models.ServiceBus
{
    public class TrackingQueueServiceBusMessage : QuirkLinkMessageBase
    {
        public TrackingQueueServiceBusMessage(
            string token, DateTime linkGenerationTime, DateTime accessTime, TimeSpan lifetimeAtAccess) 
            : base(token, linkGenerationTime)
        {
            AccessTime = accessTime;
            LifetimeAtAccess = lifetimeAtAccess;
        }

        public DateTime AccessTime { get; }
        public TimeSpan LifetimeAtAccess { get; }
    }
}
