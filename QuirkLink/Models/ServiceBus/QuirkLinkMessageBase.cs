namespace QuirkLink.Models.ServiceBus
{
    public abstract class QuirkLinkMessageBase
    {
        protected QuirkLinkMessageBase(string token, DateTime linkGenerationTime)
        {
            MessageId = Guid.NewGuid().ToString("N");
            LinkGenerationTime = linkGenerationTime;
            Token = token ?? throw new ArgumentNullException(nameof(token));
        }

        public string MessageId { get; }
        public string Token { get; }
        public DateTime LinkGenerationTime { get; }
    }
}
