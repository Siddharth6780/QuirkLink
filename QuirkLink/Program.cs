using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using QuirkLink.Extensions;
using QuirkLink.Models;
using QuirkLink.Services;
using QuirkLink.Services.Interfaces;
using QuirkLink.Services.ServiceBusListener;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            Args = args,
            WebRootPath = "wwwroot" // Enable static files serving
        });

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddOptions<QuirkLinkConfig>()
            .BindConfiguration("QuirkLinkConfig")
            .ValidateDataAnnotations()
            .ValidateOnStart();
        builder.Services.AddSingleton<ICryptoService, AES256CryptoService>();
        builder.Services.AddSingleton<IQuirkLinkService, QuirkLinkService>();
        builder.Services.AddSingleton<IQrCodeService, QrCodeService>();
        builder.Services.AddSingleton<IStorageService, RedisStorageService>();
        builder.Services.AddSingleton<IServiceBusListenerClient, ServiceBusListenerClient>();
        builder.Services.AddSingleton<IServiceBusPublisherClient, ServiceBusPublisherClient>(provider =>
        {
            var config = provider.GetRequiredService<IOptions<QuirkLinkConfig>>().Value;
            var serviceBusClient = new ServiceBusClient(config.ServiceBusConnectionString);
            var cleanupQueueSender = serviceBusClient.CreateSender(config.CleanupQueueName);
            var trackingQueueSender = serviceBusClient.CreateSender(config.TrackingQueueName);
            return new ServiceBusPublisherClient(cleanupQueueSender, trackingQueueSender);
        });
        builder.Services.AddHostedService(provider =>
        {
            var config = provider.GetRequiredService<IOptions<QuirkLinkConfig>>().Value;
            var serviceBusClient = new ServiceBusClient(config.ServiceBusConnectionString);
            var serviceBusListenerClient = provider.GetRequiredService<IServiceBusListenerClient>();
            return new TrackingServiceBusListener(serviceBusListenerClient, serviceBusClient, config.TrackingQueueName);
        });
        builder.Services.AddHostedService(provider =>
        {
            var config = provider.GetRequiredService<IOptions<QuirkLinkConfig>>().Value;
            var serviceBusClient = new ServiceBusClient(config.ServiceBusConnectionString);
            var serviceBusListenerClient = provider.GetRequiredService<IServiceBusListenerClient>();
            return new CleanupServiceBusListener(serviceBusListenerClient, serviceBusClient, config.CleanupQueueName);
        });


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Add request logging middleware
        app.UseRequestLogging();

        // Enable static files serving
        app.UseStaticFiles();
        
        // Set default route to serve index.html
        app.UseDefaultFiles();

        app.UseAuthorization();

        app.MapControllers();
        

        app.Run();
    }
}