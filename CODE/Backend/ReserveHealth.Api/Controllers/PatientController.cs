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
}