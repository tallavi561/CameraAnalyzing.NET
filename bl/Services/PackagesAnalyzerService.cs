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
                  return Task.FromResult("Analyzed Image");
            }

      }
}