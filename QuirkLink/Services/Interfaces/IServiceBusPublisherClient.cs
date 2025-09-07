namespace QuirkLink.Services.Interfaces
{
    public interface IServiceBusPublisherClient
    {
        Task ScheduleCleanupMessageAsync(string messageContent, uint expireSeconds, CancellationToken cancellationToken);

        Task TrackLinkAccessAsync(string token, LinkAccessInfo accessInfo, CancellationToken cancellationToken);
    }
}
