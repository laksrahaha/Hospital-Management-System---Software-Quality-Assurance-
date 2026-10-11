using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReserveHealth.Api.Data;
using ReserveHealth.Api.Models;

namespace ReserveHealth.Api.Controllers
{
    [Authorize(Roles = "Doctor,Lab Technician")]
    [ApiController]
    [Route("api/audit")]
    public class AuditController : ControllerBase
    {
        private readonly ReserveHealthContext _context;

        public AuditController(
            ReserveHealthContext context)
        {
            _context = context;
        }

        // Audit records are read-only.
        // There are deliberately no POST, PUT or DELETE endpoints.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuditLog>>> GetAuditLogs()
        {
            var auditLogs =
                await _context.AuditLogs
                    .AsNoTracking()
                    .OrderByDescending(
                        log => log.Timestamp)
                    .ToListAsync();

            return Ok(auditLogs);
        }
    }
}