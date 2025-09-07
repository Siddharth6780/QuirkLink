namespace QuirkLink.Models
{
    public class GenerateQuirkLinkResponseModel
    {
        public GenerateQuirkLinkResponseModel(Uri quirkLink, string qrCodeBase64, DateTime expireTime)
        {
            QuirkLink = quirkLink ?? throw new ArgumentNullException(nameof(quirkLink));
            QrCodeBase64 = qrCodeBase64 ?? throw new ArgumentNullException(nameof(qrCodeBase64));
            ExpireTime = expireTime;
        }

        public Uri QuirkLink { get; }

        public DateTime ExpireTime { get; }

        public string QrCodeBase64 { get; }
    }
}
