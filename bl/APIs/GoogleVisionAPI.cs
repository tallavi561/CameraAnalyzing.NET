using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CameraAnalyzer.bl.Utils;
using Microsoft.Extensions.Configuration;

namespace CameraAnalyzer.bl.APIs
{
    public class GoogleVisionAPI
    {
        private readonly string _apiEndpoint = "https://vision.googleapis.com/v1/images:annotate";
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public GoogleVisionAPI(IConfiguration configuration)
        {
            _apiKey = configuration["GoogleVisionAPI:ApiKey"];
            _httpClient = new HttpClient();

            if (string.IsNullOrEmpty(_apiKey))
                Logger.LogWarning("Google Vision API key not found in configuration!");
            else
                Logger.LogInfo("Google Vision API initialized successfully.");
        }

        /// <summary>
        /// Sends an image to Google Vision API for object detection or analysis.
        /// </summary>
        /// <param name="imagePath">Path to the image file on disk.</param>
        /// <param name="prompt">Instruction or analysis context (for logging purposes only).</param>
        /// <returns>Raw JSON string with the API response.</returns>


        public async Task<string> AnalyzeImageAsync(string imagePath, string prompt)
        {
            try
            {
                if (!File.Exists(imagePath))
                    throw new FileNotFoundException("Image file not found.", imagePath);

                byte[] imageBytes = await File.ReadAllBytesAsync(imagePath);
                string base64Image = Convert.ToBase64String(imageBytes);

                Logger.LogInfo($"Sending image '{Path.GetFileName(imagePath)}' to Google Vision API with prompt: {prompt}");
                Logger.LogInfo($"Sending image '{imagePath}' to Google Vision API with prompt: {prompt}");

                var requestBody = new
                {
                    requests = new[]
                    {
                        new
                        {
                            image = new { content = base64Image },
                            features = new[] { new { type = "OBJECT_LOCALIZATION" } }
                        }
                    }
                };

                string jsonBody = JsonSerializer.Serialize(requestBody);

                var response = await _httpClient.PostAsync(
                    $"{_apiEndpoint}?key={_apiKey}",
                    new StringContent(jsonBody, Encoding.UTF8, "application/json")
                );

                response.EnsureSuccessStatusCode();

                Logger.LogInfo("Google Vision API response received successfully.");
                string jsonResponse = await response.Content.ReadAsStringAsync();
                Logger.LogInfo("Google Vision API response received successfully.");

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
