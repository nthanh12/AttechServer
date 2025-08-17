using AttechServer.Applications.UserModules.Abstracts;
using AttechServer.Applications.UserModules.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttechServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TranslateController : ControllerBase
    {
        private readonly ITranslationService _translationService;
        private readonly ILogger<TranslateController> _logger;

        public TranslateController(ITranslationService translationService, ILogger<TranslateController> logger)
        {
            _translationService = translationService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Translate([FromBody] TranslateRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Text))
                {
                    return Ok(new { translatedText = "" });
                }

                _logger.LogInformation($"Translating text from {request.Source} to {request.Target}: {request.Text}");

                var translatedText = await _translationService.TranslateTextAsync(
                    request.Text,
                    request.Source,
                    request.Target
                );

                _logger.LogInformation($"Translation result: {translatedText}");

                return Ok(new { translatedText });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Translation failed");

                // Return original text as fallback
                return Ok(new { translatedText = request.Text });
            }
        }
    }
}
