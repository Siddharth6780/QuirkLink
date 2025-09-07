using Microsoft.AspNetCore.Mvc;
using QuirkLink.Models;
using QuirkLink.Services.Interfaces;

namespace QuirkLink.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QuirkLinkController : Controller
    {
        private readonly ILogger<QuirkLinkController> logger;
        private readonly IQuirkLinkService quirkLinkService;
        private readonly IQrCodeService qrCodeService;

        public QuirkLinkController(ILogger<QuirkLinkController> logger, IQuirkLinkService quirkLinkService, IQrCodeService qrCodeService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.quirkLinkService = quirkLinkService ?? throw new ArgumentNullException(nameof(quirkLinkService));
            this.qrCodeService = qrCodeService ?? throw new ArgumentNullException(nameof(qrCodeService));
        }

        [HttpPost("link")]
        public async Task<IActionResult> GenerateQuirkLink([FromBody] QuirkLinkRequestModel request, CancellationToken cancellationToken)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            var quirkLink = await quirkLinkService.GenerateQuirkLinkAsync(request.Content, request.ExpireSeconds, cancellationToken);
            var qrCode = await qrCodeService.GenerateQrCodeAsync(quirkLink.ToString());
            var response = new GenerateQuirkLinkResponseModel(quirkLink, qrCode, DateTime.UtcNow + TimeSpan.FromSeconds(request.ExpireSeconds));
            return this.Ok(response);
        }

        [HttpGet("content/{token}")]
        public async Task<IActionResult> GetQuirkLink([FromRoute] string token, CancellationToken cancellationToken)
        {
            _ = string.IsNullOrEmpty(token) ? throw new ArgumentNullException(nameof(token)) : token;
            try
            {
                var result = await quirkLinkService.DecryptQuirkLinkAsync(token, cancellationToken);
                return this.Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return this.NotFound("The token does not exist or has already been used.");
            }
        }
    }
}
