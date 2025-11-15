using CameraAnalyzer.bl.APIs;
using CameraAnalyzer.bl.Utils;

namespace CameraAnalyzer.bl.Services
{
      public interface IPackagesAnalyzerService
      {
            Task<string> AnalyzeImageAsync();
      }

      public class PackagesAnalyzerService : IPackagesAnalyzerService
      {
            private readonly GoogleVisionAPI _googleVisionAPI;
            private readonly IGeminiService _geminiService;

            public PackagesAnalyzerService(GoogleVisionAPI googleVisionAPI,
                                           IGeminiService geminiService)
            {
                  _googleVisionAPI = googleVisionAPI;
                  _geminiService = geminiService;
            }
            private string GetPromptForBoundingBoxes()
            {
                  return string.Join("\n",
                  [
                "Analyze the attached image of packages.",
                "Detect all packages visible in the image.",
                "For each detected package, return its bounding box coordinates as pixel values relative to the top-left corner of the image.",
                "Return the result strictly as a JSON array, where each element is an object in the format:",
                "[{\"x1\": <left>, \"y1\": <top>, \"x2\": <right>, \"y2\": <bottom>}]",
                "Do not include any explanations, comments, or additional text — only the JSON array."
                  ]);
            }

            public async Task<string> AnalyzeImageAsync()
            {
                  // Step 1: detect packages using Google Vision API
                  Logger.LogInfo("Starting package detection using Google Vision API...");
                  string visionResult = await _googleVisionAPI.AnalyzeImageAsync(
                      "./test1.png",
                      GetPromptForBoundingBoxes()
                  );
                  Logger.LogDebug("Google Vision API response: " + visionResult);

                  // Step 2: crop images (you implement this part)

                  // Step 3: analyze each cropped image with Gemini
                  // var geminiResponse = await _geminiService.AnalyzeAsync(croppedImage);

                  return "Analyzed Image";
            }
      }
}
