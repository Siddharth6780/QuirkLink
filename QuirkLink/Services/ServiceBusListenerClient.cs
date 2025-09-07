using Azure.Messaging.ServiceBus;
using QuirkLink.Models.ServiceBus;
using QuirkLink.Services.Interfaces;

namespace QuirkLink.Services
{
    public class ServiceBusListenerClient : IServiceBusListenerClient
    {
        private readonly ILogger<ServiceBusListenerClient> logger;
        private readonly IStorageService storageService;
        public ServiceBusListenerClient(ILogger<ServiceBusListenerClient> logger, IStorageService storageService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        }

        public async Task ProcessCleanupMessageAsync(CleanupQueueServiceBusMessage message, CancellationToken cancellationToken)
        {
            // Check if the Message Exist in the Storage Service.
            bool isExist = await this.storageService.ExistsAsync(message.Token);

            // If Exist, Log that the message is not consumed.
            if (!isExist)
            {
                this.logger.LogInformation("Message with Token: {Token} is already consumed.", message.Token);
                return;
            }

            // If Exist, Delete the message from the Storage Service.
            this.logger.LogWarning("Message with Token: {Token} is not consumed within expiry time.", message.Token);
            await this.storageService.DeleteAsync(message.Token);
        }

        public async Task ProcessLinkAccessMessageAsync(TrackingQueueServiceBusMessage message, CancellationToken cancellationToken)
        {
            // Check if the message is already deleted from the Storage Service.
            // Ideally it should already be deleted by the Cleanup Message Processor.
            // If not, Raise an Exception.
            bool isExist = await this.storageService.ExistsAsync(message.Token);

            if (isExist)
            {
                throw new InvalidOperationException($"Message with Token: {message.Token} is already consumed.");
            }

            // Future Scope: Implement Notification System.
            this.logger.LogInformation("Link with Token: {Token} accessed at {AccessTime}", message.Token, message.AccessTime);
        }
    }
}
