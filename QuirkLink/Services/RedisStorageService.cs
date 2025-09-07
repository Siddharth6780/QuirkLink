using Microsoft.Extensions.Options;
using QuirkLink.Models;
using QuirkLink.Services.Interfaces;
using StackExchange.Redis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QuirkLink.Services
{
    public class RedisStorageService : IStorageService, IDisposable
    {
        private readonly ILogger<RedisStorageService> _logger;
        private readonly QuirkLinkConfig _config;
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;
        private bool _disposed;
        private string GetFullKey(string key) => $"{_config.RedisKeyPrefix}{key}";

        public RedisStorageService(IOptions<QuirkLinkConfig> config, ILogger<RedisStorageService> logger)
        {
            _config = config?.Value ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            try
            {
                _redis = ConnectionMultiplexer.Connect(_config.RedisConnectionString);
                _database = _redis.GetDatabase();
                _logger.LogInformation("Connected to Redis at {ConnectionString}", _config.RedisConnectionString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to Redis at {ConnectionString}", _config.RedisConnectionString);
                throw;
            }
        }

        public async Task<RedisSchema?> GetAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key cannot be null or empty", nameof(key));
            }

            string redisKey = GetFullKey(key);
            
            RedisValue value = await _database.StringGetAsync(redisKey);
            if (value.IsNull)
            {
                _logger.LogInformation("Key not found in Redis");
                return null;
            }

            _logger.LogInformation("Retrieved value from Redis");
            return JsonSerializer.Deserialize<RedisSchema>(value);
        }

        public async Task SetAsync(string key, RedisSchema value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key cannot be null or empty", nameof(key));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            string redisKey = GetFullKey(key);

            try
            {
                await _database.StringSetAsync(redisKey, JsonSerializer.Serialize(value));
                this._logger.LogInformation("Set key in Redis success");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting key in Redis");
                throw;
            }
        }

        public async Task<RedisSchema?> GetAndDeleteAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key cannot be null or empty", nameof(key));
            }

            string redisKey = GetFullKey(key);
            
            try
            {
                string script = @"
                    local value = redis.call('GET', KEYS[1])
                    if value then
                        redis.call('DEL', KEYS[1])
                    end
                    return value
                ";
                
                RedisResult result = await _database.ScriptEvaluateAsync(
                    script,
                    new RedisKey[] { redisKey });

                if (result.IsNull)
                {
                    _logger.LogInformation("Key not found in Redis for GETDEL operation");
                    return null;
                }

                _logger.LogDebug("Retrieved and deleted value for key from Redis");
                return JsonSerializer.Deserialize<RedisSchema>(result.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing GETDEL for key in Redis");
                throw;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _redis?.Close();
                _redis?.Dispose();
                _logger.LogInformation("Redis connection disposed");
            }

            _disposed = true;
        }

        public async Task<bool> ExistsAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key cannot be null or empty", nameof(key));
            }
            var redisKey = GetFullKey(key);

            return await _database.KeyExistsAsync(redisKey);
        }

        public async Task DeleteAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key cannot be null or empty", nameof(key));
            }

            var redisKey = GetFullKey(key);
            await _database.KeyDeleteAsync(redisKey);
        }
    }
}
