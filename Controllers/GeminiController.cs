using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CameraAnalyzer.bl.Services;
using CameraAnalyzer.bl.Utils;

namespace CameraAnalyzer.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class GeminiController : ControllerBase
    {
        private readonly IGeminiService _geminiService;

        public GeminiController(IGeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        [HttpGet("analyzeFixedImage")]
        public async Task<IActionResult> AnalyzeFixedImageAsync([FromQuery] string? prompt)
        {
            Logger.LogInfo("GeminiController.AnalyzeFixedImageAsync called.");

            var result = await _geminiService.AnalyzeFixedImageAsync(prompt);

            if (result == null)
            {
                Logger.LogError("Service returned null result.");
                return BadRequest("Gemini service returned null result.");
            }

            return Ok(result);
        }
    }
}
