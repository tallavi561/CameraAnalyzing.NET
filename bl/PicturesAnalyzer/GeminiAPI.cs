using Microsoft.Extensions.Configuration;
using System;

namespace CameraAnalyzer.bl.PicturesAnalyzer
{
    public class GeminiAPI
    {
        private readonly string _apiKey;

        public GeminiAPI(IConfiguration config)
        {
            _apiKey = config["GeminiAPI:ApiKey"]
                      ?? Environment.GetEnvironmentVariable("GEMINI_API_KEY")
                      ?? throw new InvalidOperationException("Gemini API key not found in configuration or environment variables.");
        }

        public void PrintKey()
        {
            Console.WriteLine($"API Key: {_apiKey}");
        }
        public string GetApiKey() => _apiKey;
    }
}
