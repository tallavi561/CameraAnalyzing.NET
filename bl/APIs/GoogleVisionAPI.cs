using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CameraAnalyzer.bl.Utils;

namespace CameraAnalyzer.bl.APIs
{
    public static class GoogleVisionAPI
    {
        private static readonly string _apiEndpoint = "https://vision.googleapis.com/v1/images:annotate";
        private static readonly string _apiKey = Environment.GetEnvironmentVariable("GOOGLE_VISION_API_KEY") 
                                                 ?? "YOUR_API_KEY_HERE";

        /// <summary>
        /// Sends an image to Google Vision API for object detection or analysis.
        /// </summary>
        /// <param name="imagePath">Path to the image file on disk.</param>
        /// <param name="prompt">Instruction or analysis context (e.g. "Detect all labels").</param>
        /// <returns>Raw JSON string with the API response.</returns>
        public static async Task<string> AnalyzeImageAsync(string imagePath, string prompt)
        {
            try
            {
                if (!File.Exists(imagePath))
                    throw new FileNotFoundException("Image file not found.", imagePath);

                byte[] imageBytes = await File.ReadAllBytesAsync(imagePath);
                string base64Image = Convert.ToBase64String(imageBytes);

                // Currently Google Vision does not use "prompt" text, but we log it for future extensions.
                Logger.LogInfo($"Sending image '{imagePath}' to Google Vision API with prompt: {prompt}");

                var requestBody = new
                {
                    requests = new[]
                    {
                        new
                        {
                            image = new { content = base64Image },
                            features = new[]
                            {
                                new { type = "OBJECT_LOCALIZATION" }
                            }
                        }
                    }
                };

                string jsonBody = JsonSerializer.Serialize(requestBody);
                using var httpClient = new HttpClient();

                var response = await httpClient.PostAsync(
                    $"{_apiEndpoint}?key={_apiKey}",
                    new StringContent(jsonBody, Encoding.UTF8, "application/json")
                );

                response.EnsureSuccessStatusCode();
                string jsonResponse = await response.Content.ReadAsStringAsync();

                Logger.LogInfo($"Google Vision API response received successfully.");
                return jsonResponse;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error calling Google Vision API: {ex.Message}");
                return $"{{\"error\":\"{ex.Message}\"}}";
            }
        }
    }
}
