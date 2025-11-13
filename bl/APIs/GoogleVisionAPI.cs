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

        public GoogleVisionAPI(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;

            _apiKey = configuration["GoogleVision:ApiKey"]
                ?? throw new Exception("GoogleVision:ApiKey is missing in appsettings.json");
        }

        public async Task<string> AnalyzeImageAsync(string imagePath, string prompt)
        {
            try
            {
                if (!File.Exists(imagePath))
                    throw new FileNotFoundException("Image file not found.", imagePath);

                byte[] imageBytes = await File.ReadAllBytesAsync(imagePath);
                string base64Image = Convert.ToBase64String(imageBytes);

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
