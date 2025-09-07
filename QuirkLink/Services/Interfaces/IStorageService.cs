using QuirkLink.Models;

namespace QuirkLink.Services.Interfaces
{
    public interface IStorageService
    {
        Task<RedisSchema?> GetAsync(string key);

        Task SetAsync(string key, RedisSchema value);

        Task<RedisSchema?> GetAndDeleteAsync(string key);

        Task<bool> ExistsAsync(string key);

        Task DeleteAsync(string key);
    }
}
