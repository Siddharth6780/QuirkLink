using global::Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using QuirkLink.Models.ServiceBus;
using QuirkLink.Services.Interfaces;

namespace QuirkLink.Services
{
    public class ServiceBusPublisherClient : IServiceBusPublisherClient
    {
        private readonly ServiceBusSender cleanupQueueSender;
        private readonly ServiceBusSender trackingQueueSender;

        public ServiceBusPublisherClient(ServiceBusSender cleanupQueueSender, ServiceBusSender trackingQueueSender)
        {
            this.cleanupQueueSender = cleanupQueueSender ?? throw new ArgumentNullException(nameof(cleanupQueueSender));
            this.trackingQueueSender = trackingQueueSender ?? throw new ArgumentNullException(nameof(trackingQueueSender));
        }

        public async Task ScheduleCleanupMessageAsync(string messageContent, uint expireSeconds, CancellationToken cancellationToken)
        {
            // Create a CleanupQueueServiceBusMessage and send it to the cleanup queue with a scheduled enqueue time.
            CleanupQueueServiceBusMessage message = new CleanupQueueServiceBusMessage(
                messageContent, DateTime.UtcNow, expireSeconds);

            // Send the message to the cleanup queue with a scheduled enqueue time.
            await cleanupQueueSender.SendMessageAsync(new ServiceBusMessage(JsonConvert.SerializeObject(message))
            {
                ScheduledEnqueueTime = DateTimeOffset.UtcNow.AddSeconds(expireSeconds),
                MessageId = message.MessageId.ToString(),
            },
            cancellationToken);
        }

        public async Task TrackLinkAccessAsync(string token, LinkAccessInfo accessInfo, CancellationToken cancellationToken)
        {
            // Create a TrackingQueueServiceBusMessage and send it to the tracking queue.
            TrackingQueueServiceBusMessage message = new TrackingQueueServiceBusMessage(
                token, accessInfo.LinkGenerationTime, accessInfo.AccessTime, accessInfo.LifetimeAtAccess);

            // Send the message to the tracking queue.
            await trackingQueueSender.SendMessageAsync(
                new ServiceBusMessage(JsonConvert.SerializeObject(message)), cancellationToken);
        }
    }
}
