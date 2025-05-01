namespace DatingApp.Server.Controllers;

// HealthController.cs
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Healthy");
    }
}

