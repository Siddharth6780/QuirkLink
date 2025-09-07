using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using QuirkLink.Models;
using QuirkLink.Services.Interfaces;

namespace QuirkLink.Services
{
    public class QuirkLinkService : IQuirkLinkService
    {
        private readonly ICryptoService cryptoService;
        private readonly IStorageService storageService;
        private readonly IServiceBusPublisherClient serviceBusPublisherClient;
        private readonly IWebHostEnvironment environment;
        private readonly QuirkLinkConfig config;

        public QuirkLinkService(
            ICryptoService cryptoService, 
            IStorageService storageService, 
            IServiceBusPublisherClient serviceBusPublisherClient,
            IWebHostEnvironment environment,
            IOptions<QuirkLinkConfig> config)
        {
            this.cryptoService = cryptoService ?? throw new ArgumentNullException(nameof(cryptoService));
            this.storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
            this.serviceBusPublisherClient = serviceBusPublisherClient ?? throw new ArgumentNullException(nameof(serviceBusPublisherClient));
            this.environment = environment ?? throw new ArgumentNullException(nameof(environment));
            this.config = config?.Value ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task<QuirkLinkResponseModel> DecryptQuirkLinkAsync(string token, CancellationToken cancellationToken)
        {
            // Check if the token entry is present or not.
            bool exists = await this.storageService.ExistsAsync(token);
            if (!exists)
            {
                throw new KeyNotFoundException("The token does not exist.");
            }

            // Retrieve the encrypted content from Redis using the token.
            RedisSchema? value = await this.storageService.GetAndDeleteAsync(token) ?? throw new KeyNotFoundException();

            // Decrypt the content using the IV and return the plain text.
            var response = this.cryptoService.Decrypt(value.CipherText, value.Iv);

            // Create a LinkAccessInfo object to log the access details.
            LinkAccessInfo accessInfo = new LinkAccessInfo
            {
                LinkGenerationTime = DateTime.Parse(value.CreatedTime),
                AccessTime = DateTime.UtcNow,
                LifetimeAtAccess = (DateTime.UtcNow - DateTime.Parse(value.CreatedTime))
            };

            // Send the message to service bus for Tracking or Logging purpose.
            await this.serviceBusPublisherClient.TrackLinkAccessAsync(token, accessInfo, cancellationToken);

            // Return the decrypted content.
            return new QuirkLinkResponseModel(response);
        }

        public async Task<Uri> GenerateQuirkLinkAsync(string plainText, uint expireSeconds, CancellationToken cancellationToken)
        {
            // Encrypt the plain text.
            (string cipherText, string iv) = this.cryptoService.Encrypt(plainText);

            // Create a Redis schema to store the encrypted content along with the IV and timestamp.
            RedisSchema redisSchema = new RedisSchema(cipherText, iv, DateTime.UtcNow.ToString("o"));

            // Generate a new token(GUID) and store the encrypted content in Redis.
            string token = Guid.NewGuid().ToString("N");

            // Store the encrypted content in Redis.
            await this.storageService.SetAsync(token, redisSchema);

            // Send the message to Service Bus with delayed delivery of expiry time.
            await this.serviceBusPublisherClient.ScheduleCleanupMessageAsync(token, expireSeconds, cancellationToken);

            // Generate the appropriate URL based on the environment
            string baseUrl = GetBaseUrlForEnvironment();
            
            // Return the QuirkLink URI
            if (baseUrl.EndsWith("/"))
            {
                return new Uri($"{baseUrl}QuirkLink/content/{token}");
            }
            
            return new Uri($"{baseUrl}/QuirkLink/content/{token}");
        }

        private string GetBaseUrlForEnvironment()
        {
            if (environment.IsDevelopment() && !string.IsNullOrEmpty(config.DevelopmentBaseUrl))
            {
                return config.DevelopmentBaseUrl;
            }
            else if (environment.IsProduction() && !string.IsNullOrEmpty(config.ProductionBaseUrl))
            {
                return config.ProductionBaseUrl;
            }
            else if (environment.IsEnvironment("Test") && !string.IsNullOrEmpty(config.TestBaseUrl))
            {
                return config.TestBaseUrl;
            }
            
            // Fallback to the default scheme if no environment-specific URL is configured
            return config.DefaultScheme;
        }
    }
}
