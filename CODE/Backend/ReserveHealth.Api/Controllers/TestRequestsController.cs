using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReserveHealth.Api.Data;
using ReserveHealth.Api.Models;

namespace ReserveHealth.Api.Controllers;

public class EnterTestResultRequest
{
    public string ResultInformation { get; set; } = "";
}

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

    [HttpPost("{id:int}/result")]
    public async Task<IActionResult> EnterResult(
        int id,
        [FromBody] EnterTestResultRequest input)
    {
        if (input == null || string.IsNullOrWhiteSpace(input.ResultInformation))
        {
            return BadRequest("Result information is required.");
        }

        if (!int.TryParse(
            User.FindFirstValue(ClaimTypes.NameIdentifier),
            out var technicianId))
        {
            return Unauthorized();
        }

        var request = await _context.TestRequests.FindAsync(id);

        if (request == null)
        {
            return NotFound("Test request not found.");
        }

        if (request.Status == "Completed" ||
            await _context.TestResults.AnyAsync(
                result => result.TestRequestId == id))
        {
            return Conflict("A result has already been entered for this request.");
        }

        var result = new TestResult
        {
            TestRequestId = id,
            ResultInformation = input.ResultInformation.Trim(),
            EnteredByUserId = technicianId,
            RecordedAt = DateTime.UtcNow
        };

        _context.TestResults.Add(result);
        request.Status = "Completed";

        await _context.SaveChangesAsync();

        return Ok(new
        {
            result.TestResultId,
            result.TestRequestId,
            request.PatientId,
            request.TestType,
            result.ResultInformation,
            result.EnteredByUserId,
            result.RecordedAt,
            request.Status
        });
    }
}