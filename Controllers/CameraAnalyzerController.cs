using Microsoft.AspNetCore.Mvc;

namespace CameraAnalyzer.Controllers
{
    [ApiController]
    // ��� �� ���� ����� �"api1/v1" ���� �� �-[controller]
    [Route("api1/v1")]
    public class CameraAnalyzerController : ControllerBase
    {
        // ���� �� �-"getHomePage" ����� ������ ������
        [HttpGet("getHomePage")]
        public IActionResult GetHomePage()
        {
            return Ok("HELLO WORLD");
        }
    }
}