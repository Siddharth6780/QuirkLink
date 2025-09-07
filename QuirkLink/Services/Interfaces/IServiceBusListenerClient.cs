using QuirkLink.Models.ServiceBus;

namespace QuirkLink.Services.Interfaces
{
    public interface IServiceBusListenerClient
    {
        Task ProcessCleanupMessageAsync(CleanupQueueServiceBusMessage message, CancellationToken cancellationToken);
        Task ProcessLinkAccessMessageAsync(TrackingQueueServiceBusMessage message, CancellationToken cancellationToken);
    }
}
