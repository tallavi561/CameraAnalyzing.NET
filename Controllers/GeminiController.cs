using Microsoft.AspNetCore.Mvc;
using CameraAnalyzer.bl.APIs;
using CameraAnalyzer.bl.Utils;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CameraAnalyzer.Controllers
{
    [ApiController]
    [Route("api1/v1")]
    public class GeminiController : ControllerBase
    {
        private readonly GeminiAPI _geminiAPI;

        public GeminiController(GeminiAPI geminiAPI)
        {
            _geminiAPI = geminiAPI;
        }

        // -----------------------------------------------------------
        //  TEXT-ONLY TEST
        // -----------------------------------------------------------
        [HttpGet("askGemini")]
        public async Task<IActionResult> AskGeminiAsync([FromQuery] string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                return BadRequest("Prompt cannot be empty.");

            Logger.Instance.LogInfo($"AskGemini endpoint accessed with prompt: {prompt}");

            string? result = await _geminiAPI.AskGeminiAsync(prompt);

            if (string.IsNullOrWhiteSpace(result))
            {
                Logger.Instance.LogError("Gemini returned empty response.");
                return BadRequest("Error calling Gemini API.");
            }

            Logger.Instance.LogInfo("Gemini text response received successfully.");
            return Ok(result);
        }

        // -----------------------------------------------------------
        //  FIXED IMAGE ANALYSIS (FOR TESTING)
        // -----------------------------------------------------------
        [HttpGet("analyzeFixedImage")]
        public async Task<IActionResult> AnalyzeFixedImageAsync([FromQuery] string? prompt)
        {
            prompt ??= "Analyze this image.";

            string imagePath = "./pictures/RS0933030576Y.jpg";

            if (!System.IO.File.Exists(imagePath))
            {
                Logger.Instance.LogError($"Image not found at: {imagePath}");
                return NotFound($"File '{imagePath}' not found.");
            }

            // exactly like your example
            string base64Image;
            using (var memoryStream = new MemoryStream())
            {
                await using (var fileStream = System.IO.File.OpenRead(imagePath))
                    await fileStream.CopyToAsync(memoryStream);
                base64Image = Convert.ToBase64String(memoryStream.ToArray());
            }

            string mimeType = "image/png";
            Logger.Instance.LogInfo($"Sending fixed image '{imagePath}' to Gemini API...");

            string? result = await _geminiAPI.AnalyzeImageAsync(base64Image, mimeType);

            if (string.IsNullOrWhiteSpace(result))
            {
                Logger.Instance.LogError("Gemini returned empty response for image.");
                return BadRequest("Gemini returned no text output.");
            }

            Logger.Instance.LogInfo("Gemini image analysis completed successfully: " + result);
            return Ok(result);
        }
    }
}
