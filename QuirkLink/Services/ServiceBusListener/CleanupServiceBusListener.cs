
using Azure.Messaging.ServiceBus;
using QuirkLink.Models.ServiceBus;
using QuirkLink.Services.Interfaces;

namespace QuirkLink.Services.ServiceBusListener
{
    public class CleanupServiceBusListener : BackgroundService
    {
        private readonly IServiceBusListenerClient serviceBusListenerClient;
        private readonly ServiceBusClient serviceBusClient;
        private readonly string QueueName;
        private ServiceBusProcessor processor;

        public CleanupServiceBusListener(IServiceBusListenerClient serviceBusListenerClient, ServiceBusClient serviceBusClient, string queueName)
        {
            this.serviceBusListenerClient = serviceBusListenerClient ?? throw new ArgumentNullException(nameof(serviceBusListenerClient));
            this.serviceBusClient = serviceBusClient ?? throw new ArgumentNullException(nameof(serviceBusClient));
            this.QueueName = queueName ?? throw new ArgumentNullException(nameof(queueName));
            this.processor = this.serviceBusClient.CreateProcessor(this.QueueName, new ServiceBusProcessorOptions());
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            processor.ProcessMessageAsync += MessageHandler;
            processor.ProcessErrorAsync += ErrorHandler;

            await this.processor.StartProcessingAsync(stoppingToken);
        }

        public async Task MessageHandler(ProcessMessageEventArgs args)
        {
            try
            {
                // Deserialize the message body.
                _ = args?.Message?.Body ?? throw new ArgumentNullException(nameof(args));
                CleanupQueueServiceBusMessage message = args.Message.Body.ToObjectFromJson<CleanupQueueServiceBusMessage>();

                // Process the message.
                _ = message ?? throw new ArgumentNullException(nameof(message));
                await this.serviceBusListenerClient.ProcessCleanupMessageAsync(message, CancellationToken.None);

                // Complete the message. Message is deleted from the queue.
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception)
            {
                // Abandon the message in case of any exception.
                await args.AbandonMessageAsync(args.Message);
            }
        }

        public Task ErrorHandler(ProcessErrorEventArgs args)
        {
            return Task.CompletedTask;
        }
    }
}
