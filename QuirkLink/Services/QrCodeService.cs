using QRCoder;
using QuirkLink.Services.Interfaces;

namespace QuirkLink.Services
{
    public class QrCodeService : IQrCodeService
    {
        public QrCodeService()
        {
        }
        public Task<string> GenerateQrCodeAsync(string content)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeAsPngByteArr = qrCode.GetGraphic(5);
            string base64String = Convert.ToBase64String(qrCodeAsPngByteArr, 0, qrCodeAsPngByteArr.Length);
            return Task.FromResult(base64String);
        }
    }
}
