using Microsoft.AspNetCore.Mvc;
using CameraAnalyzer.bl.Utils;
using Microsoft.Extensions.Configuration;
using CameraAnalyzer.bl.APIs;

namespace CameraAnalyzer.Controllers
{
    [ApiController]
    [Route("api1/v1")]
    public class CameraAnalyzerController : ControllerBase
    {


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
            return Ok("Process Finished");
        }
    }
}