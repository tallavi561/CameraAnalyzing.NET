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

        public CameraAnalyzerController(IPackagesAnalyzerService packagesAnalyzerService)
        {
            _packagesAnalyzerService = packagesAnalyzerService;
        }

        [HttpGet("getHomePage")]
        public IActionResult GetHomePage()
        {
            Logger.LogInfo("Home page accessed.");
            return Ok("HELLO WORLD");
        }

        [HttpGet("startProcess")]
        public async Task<IActionResult> StartProcess()
        {
            Logger.LogInfo("Start process is starting.");
            string result = await _packagesAnalyzerService.AnalyzeImageAsync();
            return Ok("Process Finished " + result);
        }
    }
}