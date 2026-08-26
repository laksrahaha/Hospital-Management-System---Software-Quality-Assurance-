using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReserveHealth.Api.Data;
using ReserveHealth.Api.Models;

namespace ReserveHealth.Api.Controllers;

// This controller is used to get patient information from the database.
// I added a single patient request because the dashboard will need to
// open one patient's information instead of only showing the full list.
[Route("api/[controller]")]
[ApiController]
public class PatientController : ControllerBase
{
    private readonly ReserveHealthContext _context;

    public PatientController(ReserveHealthContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Patient>>> GetPatients()
    {
        return await _context.Patients.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Patient>> GetPatient(int id)
    {
        var patient = await _context.Patients.FindAsync(id);

        if (patient == null)
        {
            return NotFound();
        }

        return patient;
    }
//copilot prompt added for new patients
    [HttpPost]
    public async Task<ActionResult<Patient>> CreatePatient(Patient patient)
    {
        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetPatient),
            new { id = patient.PatientId },
            patient);
    }

//copilot prompt added
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePatient(int id, Patient patient)
    {
        if (id != patient.PatientId)
        {
            return BadRequest("Patient ID does not match the route ID.");
        }

        var existingPatient = await _context.Patients.FindAsync(id);

        if (existingPatient == null)
        {
            return NotFound();
        }

        _context.Entry(existingPatient).CurrentValues.SetValues(patient);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}