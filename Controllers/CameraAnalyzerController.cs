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
        private readonly GeminiAPI _geminiAPI; // ← שדה פרטי של GeminiAPI

        public static Logger logger = new Logger();
        public CameraAnalyzerController(GeminiAPI geminiAPI)
        {
            _geminiAPI = geminiAPI;
        }
        [HttpGet("getHomePage")]
        public IActionResult GetHomePage()
        {
            logger.LogInfo("The API key is: " + _geminiAPI.getApiKey());
            logger.LogInfo("Home page accessed.");
            return Ok("HELLO WORLD");
        }
        [HttpGet("startProcess")]
        public IActionResult StartProcess()
        {
            logger.LogInfo("Start process is starting.");
            return Ok("Process Finished");
        }
    }
}