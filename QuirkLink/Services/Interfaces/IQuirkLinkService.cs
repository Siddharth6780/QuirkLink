using QuirkLink.Models;

namespace QuirkLink.Services.Interfaces
{
    public interface IQuirkLinkService
    {
        public Task<Uri> GenerateQuirkLinkAsync(string plainText, uint expireSeconds, CancellationToken cancellationToken);

        public Task<QuirkLinkResponseModel> DecryptQuirkLinkAsync(string token, CancellationToken cancellationToken);
    }
}
