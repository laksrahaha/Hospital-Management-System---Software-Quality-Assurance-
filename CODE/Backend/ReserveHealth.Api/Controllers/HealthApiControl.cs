using Microsoft.AspNetCore.Mvc;

namespace ReserveHealth.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult GetHealth()
    {
        return Ok(new
        {
            status = "Reserve Health API is up and running"
        });
    }
}