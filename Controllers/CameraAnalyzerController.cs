using Microsoft.AspNetCore.Mvc;
using CameraAnalyzer.bl.Utils;
using Microsoft.Extensions.Configuration;
using CameraAnalyzer.bl.APIs;
using CameraAnalyzer.bl.Services;

namespace CameraAnalyzer.Controllers
{
    [ApiController]
    [Route("api1/v1/[controller]")]
    public class CameraAnalyzerController : ControllerBase
    {
        private readonly IPackagesAnalyzerService _packagesAnalyzerService;

        public CameraAnalyzerController(PackagesAnalyzerService packagesAnalyzerService)
        {
            _packagesAnalyzerService = packagesAnalyzerService;
        }

        [HttpGet("getHomePage")]
        public IActionResult GetHomePage()
        {
            // Logger.LogInfo("The API key is: " + _geminiAPI.getApiKey());
            Logger.LogInfo("Home page accessed.");
            return Ok("HELLO WORLD");
        }
        [HttpGet("startProcess")]
        public IActionResult StartProcess()
        {
            Logger.LogInfo("Start process is starting.");
            Task<string> result = _packagesAnalyzerService.AnalyzeImageAsync();
            return Ok("Process Finished " + result.Result);
        }
        
    }
}