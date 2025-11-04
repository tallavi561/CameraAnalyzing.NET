using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CameraAnalyzer.bl.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using CameraAnalyzer.bl.APIs;
using CameraAnalyzer.bl.Utils;
using System;
using System.IO;
using System.Threading.Tasks;

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

            var requestUri = $"{_apiEndpoint}/{_modelName}:generateContent?key={_apiKey}";
            var requestPayload = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var jsonPayload = JsonSerializer.Serialize(requestPayload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(requestUri, content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseBody);

                if (!doc.RootElement.TryGetProperty("candidates", out var candidates) ||
                    candidates.GetArrayLength() == 0)
                    return "No valid response from Gemini.";

                var resultText = candidates[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                return resultText ?? "No text in response.";
            }
            catch (Exception ex)
            {
                return $"Error calling Gemini API: {ex.Message}";
            }
        }


        public async Task<string?> AnalyzeImageAsync(string base64ImageData, string prompt, string mimeType = "image/jpeg")
        {
            if (string.IsNullOrWhiteSpace(base64ImageData))
            {
                throw new ArgumentException("Image data cannot be empty.", nameof(base64ImageData));
            }


            Console.WriteLine("Generated prompt for image analysis.");
            var requestUri = $"{_apiEndpoint}/{_modelName}:generateContent?key={_apiKey}";

            var requestPayload = new
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
                                    mimeType = mimeType,
                                    data = base64ImageData
                                }
                            }
                        }
                    }
                }
            };

            var jsonPayload = JsonSerializer.Serialize(requestPayload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(requestUri, content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error analyzing image: {ex.Message}");
                return null;
            }
        }

        public async Task<string?> AnalyzeImageFromStorageAsync(string imagePath, string prompt, string mimeType = "image/jpeg")
        {

            // exactly like your example
            string base64Image;
            using (var memoryStream = new MemoryStream())
            {
                await using (var fileStream = System.IO.File.OpenRead(imagePath))
                    await fileStream.CopyToAsync(memoryStream);
                base64Image = Convert.ToBase64String(memoryStream.ToArray());
            }
            if (string.IsNullOrWhiteSpace(base64Image))
            {
                throw new ArgumentException("Image data cannot be empty.", nameof(base64Image));
            }


            Console.WriteLine("Generated prompt for image analysis.");
            var requestUri = $"{_apiEndpoint}/{_modelName}:generateContent?key={_apiKey}";

            var requestPayload = new
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
                                    mimeType = mimeType,
                                    data = base64Image
                                }
                            }
                        }
                    }
                }
            };

            var jsonPayload = JsonSerializer.Serialize(requestPayload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(requestUri, content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error analyzing image: {ex.Message}");
                return null;
            }
        }





        private string GetPrompt()
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
    }
}
