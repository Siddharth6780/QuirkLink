using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Options;
using QuirkLink.Models;
using QuirkLink.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace QuirkLink.Services
{
    public class AES256CryptoService : ICryptoService
    {
        private readonly QuirkLinkConfig config;
        private readonly ILogger<AES256CryptoService> logger;
        private readonly byte[] encryptionKey;

        public AES256CryptoService(IOptions<QuirkLinkConfig> configOptions, ILogger<AES256CryptoService> logger)
        {
            this.config = configOptions?.Value ?? throw new ArgumentNullException(nameof(configOptions));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            string keyString = this.config.Aes256Key ?? throw new ArgumentNullException(nameof(this.config.Aes256Key));

            try
            {
                // Try to decode the key as Base64
                encryptionKey = Convert.FromBase64String(keyString);
                
                // Validate key length (32 bytes = 256 bits for AES-256)
                if (encryptionKey.Length != 32)
                {
                    throw new ArgumentException($"Invalid key length. Expected 32 bytes (256 bits) for AES-256, but got {encryptionKey.Length} bytes.");
                }
                
                logger.LogInformation("AES-256 encryption key loaded successfully");
            }
            catch (FormatException)
            {
                // If not Base64, try using the raw string (for backward compatibility)
                if (string.IsNullOrEmpty(keyString) || keyString.Length != 32)
                {
                    throw new ArgumentException("Key must be either a valid Base64 string that decodes to 32 bytes, or a 32-character string for AES-256.");
                }
                
                encryptionKey = Encoding.UTF8.GetBytes(keyString);
                logger.LogInformation("AES-256 encryption key loaded from raw string");
            }
        }

        public (string cipherText, string iv) Encrypt(string plainText)
        {
            // Implementation for AES-256 encryption
            using var aes = Aes.Create();
            aes.Key = encryptionKey;
            aes.GenerateIV();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            return (Convert.ToBase64String(encryptedBytes), Convert.ToBase64String(aes.IV));
        }
        public string Decrypt(string cipherText, string iv)
        {
            // Implementation for AES-256 decryption
            using var aes = Aes.Create();
            aes.Key = encryptionKey;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var decryptor = aes.CreateDecryptor(aes.Key, Convert.FromBase64String(iv));
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            byte[] decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}
