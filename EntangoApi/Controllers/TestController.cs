using Microsoft.AspNetCore.Mvc;

namespace EntangoApi.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("info")]
    public IActionResult Get()
    {
        return Ok("Hello, this is Entango API. A set of APIs for various services. You can find API for: \r\n1) SMS sending (general),  \r\n2) Email sending (general),  \r\n3) Document data extraction using Gemini API (general),  \r\n4) Weather Forecast (construction sector),  \r\n5) Price List management (construction sector)");
    }
}