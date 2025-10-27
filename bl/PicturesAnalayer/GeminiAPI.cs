using Microsoft.Extensions.Configuration;

namespace CameraAnalyzer.bl.PicturesAnalayer
{
      public class GeminiAPI
      {
            private readonly string _apiKey;
            public GeminiAPI(String apiKey)
            {
                  _apiKey = apiKey;
            }
      }
}