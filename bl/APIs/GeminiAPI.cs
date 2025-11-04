using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace CameraAnalyzer.bl.APIs
{
    public class GeminiAPI
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private readonly string _apiEndpoint = "https://generativelanguage.googleapis.com/v1beta/models";
        private readonly string _modelName = "gemini-2.0-flash";

        public GeminiAPI(IConfiguration configuration)
        {
            _apiKey = configuration["GeminiAPI:ApiKey"]
                ?? throw new InvalidOperationException("Missing Gemini API key in configuration.");

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }


        public async Task<string?> AskGeminiAsync(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                throw new ArgumentException("Prompt cannot be empty.", nameof(prompt));

            var payload = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[] { new { text = prompt } }
                    }
                }
            };

            return await SendRequestAsync(payload);
        }


        public async Task<string?> AnalyzeImageAsync(string base64ImageData, string prompt, string mimeType = "image/jpeg")
        {
            if (string.IsNullOrWhiteSpace(base64ImageData))
                throw new ArgumentException("Image data cannot be empty.", nameof(base64ImageData));

            var payload = BuildImagePayload(prompt, base64ImageData, mimeType);
            return await SendRequestAsync(payload);
        }


        public async Task<string?> AnalyzeImageFromStorageAsync(string imagePath, string prompt, string mimeType = "image/jpeg")
        {
            if (!File.Exists(imagePath))
                throw new FileNotFoundException("Image file not found.", imagePath);

            string base64Image;
            await using (var fileStream = File.OpenRead(imagePath))
            using (var memoryStream = new MemoryStream())
            {
                await fileStream.CopyToAsync(memoryStream);
                base64Image = Convert.ToBase64String(memoryStream.ToArray());
            }

            var payload = BuildImagePayload(prompt, base64Image, mimeType);
            return await SendRequestAsync(payload);
        }


        private object BuildImagePayload(string prompt, string base64ImageData, string mimeType)
        {
            return new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new object[]
                        {
                            new { text = prompt },
                            new
                            {
                                inlineData = new
                                {
                                    mimeType,
                                    data = base64ImageData
                                }
                            }
                        }
                    }
                }
            };
        }

        private async Task<string?> SendRequestAsync(object payload)
        {
            var requestUri = $"{_apiEndpoint}/{_modelName}:generateContent?key={_apiKey}";
            var jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(requestUri, content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                return ParseResponse(responseBody);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calling Gemini API: {ex.Message}");
                return null;
            }
        }

        private string? ParseResponse(string responseBody)
        {
            using var doc = JsonDocument.Parse(responseBody);

            if (!doc.RootElement.TryGetProperty("candidates", out var candidates) ||
                candidates.GetArrayLength() == 0)
            {
                Console.WriteLine("Unexpected response structure from Gemini.");
                return null;
            }

            var text = candidates[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            if (string.IsNullOrWhiteSpace(text))
                return null;

            text = text.Trim();
            if (text.StartsWith("```json", StringComparison.OrdinalIgnoreCase))
                text = text.Substring(7).Trim();
            if (text.EndsWith("```"))
                text = text.Substring(0, text.Length - 3).Trim();

            return text;
        }


    }
}
