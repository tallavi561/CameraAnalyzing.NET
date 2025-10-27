using Microsoft.AspNetCore.Mvc;
using CameraAnalyzer.bl.Utils;

namespace CameraAnalyzer.Controllers
{
    [ApiController]
    [Route("api1/v1")]
    public class CameraAnalyzerController : ControllerBase
    {
    public static Logger logger = new Logger();
        [HttpGet("getHomePage")]
        public IActionResult GetHomePage()
        {
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