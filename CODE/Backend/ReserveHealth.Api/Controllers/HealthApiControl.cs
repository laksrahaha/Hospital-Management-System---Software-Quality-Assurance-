using Microsoft.AspNetCore.Mvc;

namespace ReserveHealth.Api.Controllers;

// This controller is just used to check that the Reserve Health API is running.
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