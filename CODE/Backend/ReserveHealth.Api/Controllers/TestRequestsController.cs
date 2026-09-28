using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReserveHealth.Api.Data;

namespace ReserveHealth.Api.Controllers;

[ApiController]
[Route("api/test-requests")]
[Authorize(Roles = "Lab Technician")]
public class TestRequestsController : ControllerBase
{
    private readonly ReserveHealthContext _context;

    public TestRequestsController(ReserveHealthContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetTestRequests()
    {
        var requests = await _context.TestRequests
            .AsNoTracking()
            .OrderBy(request => request.Status == "Completed")
            .ThenBy(request => request.RequestedAt)
            .Select(request => new
            {
                request.TestRequestId,
                request.PatientId,
                PatientName = request.Patient.FirstName + " " + request.Patient.LastName,
                request.TestType,
                request.Status,
                request.RequestedAt
            })
            .ToListAsync();

        return Ok(requests);
    }
}