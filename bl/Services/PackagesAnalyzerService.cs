using System.Text.Json;
using CameraAnalyzer.bl.APIs;
using CameraAnalyzer.bl.Models;
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
            private readonly GeminiAPI _geminiApi;

            public PackagesAnalyzerService(GoogleVisionAPI googleVisionAPI,
                                           GeminiAPI geminiApi)
            {
                  _googleVisionAPI = googleVisionAPI;
                  _geminiApi = geminiApi;
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
                  // string visionResult = await _googleVisionAPI.AnalyzeImageAsync(
                  //     "./test1.png",
                  //     GetPromptForBoundingBoxes()
                  // );

                  string visionResult =  await _geminiApi.AnalyzeImageFromStorageAsync(
                          "./test1.png",
                          GetPromptForBoundingBoxes()
                  );
                  Logger.LogDebug("Google Vision API response: " + visionResult);

                  // Step 2: crop images (you implement this part)
                  List<BoundingBox> boundingBoxes;
                  try
                  {
                        boundingBoxes = JsonSerializer.Deserialize<List<BoundingBox>>(visionResult);
                  }
                  catch (Exception ex)
                  {
                        Logger.LogError("Error parsing bounding box JSON: " + ex.Message);
                        return "Error parsing bounding box JSON.";
                  }

                  if (boundingBoxes == null || boundingBoxes.Count == 0)
                  {
                        Logger.LogInfo("No bounding boxes detected.");
                        return "No bounding boxes detected.";
                  }
                  Logger.LogInfo($"Detected {boundingBoxes.Count} bounding boxes. Starting cropping...");
                  foreach (var box in boundingBoxes)
                  {
                        string newFilePath = $"./cropped_outputs/crop_{Guid.NewGuid()}.png";
                        ImagesProcessing.CropAndSaveImage(box.x1, box.y1, box.x2, box.y2, "./test1.png", newFilePath);
                  }

                  // Step 3: analyze each cropped image with Gemini
                  // var geminiResponse = await _geminiService.AnalyzeAsync(croppedImage);

                  return "Analyzed Image";
            }
      }
}
