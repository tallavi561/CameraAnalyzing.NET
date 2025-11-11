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
            [
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
            ];

            return string.Join("\n", lines);
        }
        private string GetPromptForBoundingBoxes()
        {
            return string.Join("\n", new[]
            {
        "Analyze the attached image of packages.",
        "Detect all packages visible in the image.",
        "For each detected package, return its bounding box coordinates as pixel values relative to the top-left corner of the image.",
        "Return the result strictly as a JSON array, where each element is an object in the format:",
        "[{\"x1\": <left>, \"y1\": <top>, \"x2\": <right>, \"y2\": <bottom>}]",
        "Do not include any explanations, comments, or additional text — only the JSON array."
    });
        }



        [HttpGet("askGemini")]
        public async Task<IActionResult> AskGeminiAsync([FromQuery] string prompt)
        {
            string promptToUse = prompt;
            if (string.IsNullOrWhiteSpace(prompt))
            {
                promptToUse = GetPrompt();
            }

            Logger.LogInfo($"AskGemini endpoint accessed with prompt: {promptToUse}");

            string? result = await _geminiAPI.AskGeminiAsync(promptToUse);

            if (string.IsNullOrWhiteSpace(result))
            {
                Logger.LogError("Gemini returned empty response.");
                return BadRequest("Error calling Gemini API.");
            }

            Logger.LogInfo("Gemini text response received successfully.");
            return Ok(result);
        }


        [HttpGet("analyzeFixedImage")]
        public async Task<IActionResult> AnalyzeFixedImageAsync([FromQuery] string? prompt)
        {
            
            prompt ??= GetPromptForBoundingBoxes();

            string imagePath = "./test1.png";

            if (!System.IO.File.Exists(imagePath))
            {
                Logger.LogError($"Image not found at: {imagePath}");
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
            Logger.LogInfo($"Sending fixed image '{imagePath}' to Gemini API...");

            string? result = await _geminiAPI.AnalyzeImageAsync(base64Image, prompt, mimeType);

            if (string.IsNullOrWhiteSpace(result))
            {
                Logger.LogError("Gemini returned empty response for image. " + result);
                return BadRequest("Gemini returned no text output. " + result);
            }

            Logger.LogInfo("Gemini image analysis completed successfully: " + result);

            try
            {
                var boxes = System.Text.Json.JsonSerializer.Deserialize<List<CameraAnalyzer.bl.Models.BoundingBox>>(result);

                if (boxes == null || boxes.Count == 0)
                {
                    Logger.LogError("No bounding boxes found in Gemini response.");
                    return Ok("No bounding boxes detected.");
                }

                string sourceImagePath = "./test1.png";
                int index = 1;

                foreach (var box in boxes)
                {
                    string newFilePath = $"./cropped_outputs/crop_{index}.png";
                    ImagesCropper.CropAndSaveImage(box.x1, box.y1, box.x2, box.y2, sourceImagePath, newFilePath);
                    index++;
                }

                return Ok(new { message = "Cropping completed.", count = boxes.Count });
            }
            catch (Exception ex)
            {
                Logger.LogError("Error parsing bounding box JSON: " + ex.Message);
                return BadRequest("Invalid bounding box JSON format.");
            }

        }
    }
}
