namespace QuirkLink.Services.Interfaces
{
    public interface IQrCodeService
    {
        public Task<string> GenerateQrCodeAsync(string content);
    }
}
