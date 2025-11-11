using Microsoft.Extensions.Configuration;
using System;

namespace CameraAnalyzer.bl.APIs
{

    public class GeminiAPI
    {
        private readonly HttpClient _client;
        private readonly string _apiKey;

        public GeminiAPI(IConfiguration config)
        {
            _apiKey = config["GeminiAPI:ApiKey"] ?? throw new Exception("Missing Gemini API key");
            _client = new HttpClient();
        }

        public async Task<string> AskGeminiAsync(string prompt)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.gemini.com/v1/query");
            request.Headers.Add("Authorization", $"Bearer {_apiKey}");
            request.Content = new StringContent($"{{\"prompt\":\"{prompt}\"}}", System.Text.Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }
    }
}