namespace QuirkLink.Models
{
    public class QuirkLinkResponseModel
    {
        public QuirkLinkResponseModel(string message)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public string Message { get; }
    }
}
