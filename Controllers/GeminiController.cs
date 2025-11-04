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

        private string ItaysGetPrompt()
        {
            string[] lines =
            {
                "Analyze this shipping label image.",
                "Extract the 'ship to' and 'ship from' details as JSON objects.",
                "If multiple labels are detected, return an array of JSON objects.",
                "Do not add data that isn't visible in the image.",
                "If any data is missing, fill it as null.",
                "If countries/states are abbreviations, expand them to full names.",
                "For each phone number, include the correct country code (e.g. '+1', '+44').",
                "Ensure accuracy between 'to' and 'from' — if the text is near 'To:' or 'From:', it belongs there."
            };
            return string.Join("\n", lines);
        }
        private string GetPrompt()
        {
            string[] lines =
            {
        "You are a precise data extraction system that analyzes shipping label images.",
        "Your task is to extract all visible shipment information and return it **strictly** as a JSON object following this schema:",
        "",
        "interface Address {",
        "  country: string | null;",
        "  state: string | null;",
        "  region: string | null;",
        "  city: string | null;",
        "  postalCode: string | null;",
        "  streetAndHouse: string | null;",
        "}",
        "",
        "interface Details {",
        "  name: string | null;",
        "  phone: string | null;",
        "  email: string | null;",
        "  address: Address;",
        "}",
        "",
        "interface PackageDetails {",
        "  barcode: string | null;",
        "  from: Details;",
        "  to: Details;",
        "  weight: number | null;",
        "  date: string | null;",
        "  contentDescription: string[];",
        "}",
        "",
        "Instructions:",
        "1. Analyze all visible text, barcodes, and numbers in the provided image(s).",
        "2. If multiple shipping labels are visible, return an array of PackageDetails objects; otherwise, return a single object.",
        "3. Assign data accurately — fields near 'TO', 'SHIP TO', or destination areas belong to 'to'; fields near 'FROM' or sender areas belong to 'from'.",
        "4. Expand country and state abbreviations to full names (e.g., 'US' → 'United States', 'CA' → 'California').",
        "5. Normalize phone numbers to include international dialing codes (e.g., '+1', '+44').",
        "6. Use null for missing or illegible data — do not guess or infer beyond visible information.",
        "7. Detect and decode any printed barcode text into the 'barcode' field.",
        "8. Detect and list product descriptions or contents in 'contentDescription' as an array of strings.",
        "9. Output only raw JSON — no commentary, markdown, or explanations."
    };

            return string.Join("\n", lines);
        }



        [HttpGet("askGemini")]
        public async Task<IActionResult> AskGeminiAsync([FromQuery] string prompt)
        {
            string promptToUse = prompt;
            if (string.IsNullOrWhiteSpace(prompt))
            {
                promptToUse = GetPrompt();
            }

            Logger.Instance.LogInfo($"AskGemini endpoint accessed with prompt: {promptToUse}");

            string? result = await _geminiAPI.AskGeminiAsync(promptToUse);

            if (string.IsNullOrWhiteSpace(result))
            {
                Logger.Instance.LogError("Gemini returned empty response.");
                return BadRequest("Error calling Gemini API.");
            }

            Logger.Instance.LogInfo("Gemini text response received successfully.");
            return Ok(result);
        }


        [HttpGet("analyzeFixedImage")]
        public async Task<IActionResult> AnalyzeFixedImageAsync([FromQuery] string? prompt)
        {
            prompt ??= GetPrompt();

            string imagePath = "./pictures/RS0933030576Y.jpg";

            if (!System.IO.File.Exists(imagePath))
            {
                Logger.Instance.LogError($"Image not found at: {imagePath}");
                return NotFound($"File '{imagePath}' not found.");
            }


            string base64Image;
            using (var memoryStream = new MemoryStream())
            {
                await using (var fileStream = System.IO.File.OpenRead(imagePath))
                    await fileStream.CopyToAsync(memoryStream);
                base64Image = Convert.ToBase64String(memoryStream.ToArray());
            }

            string mimeType = "image/png";
            Logger.Instance.LogInfo($"Sending fixed image '{imagePath}' to Gemini API...");

            string? result = await _geminiAPI.AnalyzeImageAsync(base64Image, prompt, mimeType);

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
