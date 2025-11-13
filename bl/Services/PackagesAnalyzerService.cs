namespace CameraAnalyzer.bl.Services
{
      public interface IPackagesAnalyzerService
      {
            Task<string> AnalyzeImageAsync();
      }

      public class PackagesAnalyzerService : IPackagesAnalyzerService
      {
            public Task<string> AnalyzeImageAsync()
            {
                  // First step: detect packages in the image
                  
                  // Second step: crop detected packages

                  // Third step: analyze each cropped image with Gemini API
                  return Task.FromResult("Analyzed Image");
            }

      }
}