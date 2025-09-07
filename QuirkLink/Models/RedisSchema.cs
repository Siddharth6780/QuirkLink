
namespace QuirkLink.Models
{
    public class RedisSchema
    {
        public RedisSchema(string cipherText, string iv, string createdTime)
        {
            CipherText = cipherText ?? throw new ArgumentNullException(nameof(cipherText));
            Iv = iv ?? throw new ArgumentNullException(nameof(iv));
            CreatedTime = createdTime ?? throw new ArgumentNullException(nameof(createdTime));
        }

        public string CipherText { get; }
        public string Iv { get; }
        public string CreatedTime { get; }
    }
}
