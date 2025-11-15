using System.Text.Json;
using CameraAnalyzer.bl.APIs;
using CameraAnalyzer.bl.Models;
using CameraAnalyzer.bl.Utils;
using YoloDotNet;
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
            private readonly YoloAPI _yolo;

            public PackagesAnalyzerService(GoogleVisionAPI googleVisionAPI,
                                           GeminiAPI geminiApi)
            {
                  _googleVisionAPI = googleVisionAPI;
                  _geminiApi = geminiApi;
                  _yolo = new YoloAPI("models/yolov11n.onnx");
            }
            private string GetPromptForBoundingBoxes()
            {
                  return string.Join("\n",
                  [
                "Analyze the attached image of packages.",
                "Detect all packages visible in the image.",
                "For each detected package, return its bounding box coordinates as pixel values relative to the top-left corner of the image.",
                "Return the result strictly as a JSON array, where each element is an object in the format:",
                "[{\"X1\": <left>, \"Y1\": <top>, \"X2\": <right>, \"Y2\": <bottom>}]",
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

                  List<BoundingBox> boundingBoxes =   _yolo.Detect("./test1.png");


                  if (boundingBoxes == null || boundingBoxes.Count == 0)
                  {
                        Logger.LogInfo("No bounding boxes detected.");
                        return "No bounding boxes detected.";
                  }
                  Logger.LogInfo($"Detected {boundingBoxes.Count} bounding boxes. Starting cropping...");
                  // Step 2: crop images (you implement this part)
                  foreach (var box in boundingBoxes)
                  {
                        string newFilePath = $"./cropped_outputs/crop_{Guid.NewGuid()}.png";
                        ImagesProcessing.CropAndSaveImage(box.X1, box.Y1, box.X2, box.Y2, "./test1.png", newFilePath);
                  }

                  // Step 3: analyze each cropped image with Gemini
                  // var geminiResponse = await _geminiService.AnalyzeAsync(croppedImage);

                  return "Analyzed Image";
            }
      }
}
