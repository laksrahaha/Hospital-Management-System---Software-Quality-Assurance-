// Referral controller
// Handles creating, viewing and updating referrals.
// Uses the existing patient database to make sure
// each referral is linked to a real patient.

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReserveHealth.Api.Data;
using ReserveHealth.Api.Models;

namespace ReserveHealth.Api.Controllers
{
    public class UpdateReferralRequest
    {
        public string Priority { get; set; } = "";

        public string Status { get; set; } = "";

        public string? StatusReason { get; set; }
    }

    [ApiController]
    [Route("api/referrals")]
    public class ReferralController : ControllerBase
    {
        private readonly ReserveHealthContext _context;

        public ReferralController(ReserveHealthContext context)
        {
            _context = context;
        }

        // Get all referrals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Referral>>> GetReferrals()
        {
            var referrals =
                await _context.Referrals
                    .ToListAsync();

            return Ok(referrals);
        }

        // Create a new referral
        [HttpPost]
        public async Task<ActionResult<Referral>> CreateReferral(
            Referral referral)
        {
            if (referral.PatientId <= 0)
            {
                return BadRequest("Patient is required.");
            }

            // Check that the selected patient actually exists
            bool patientExists =
                await _context.Patients
                    .AnyAsync(
                        p => p.PatientId == referral.PatientId
                    );

            if (!patientExists)
            {
                return BadRequest(
                    "Selected patient does not exist."
                );
            }

            if (string.IsNullOrWhiteSpace(referral.Reason))
            {
                return BadRequest(
                    "Reason is required."
                );
            }

            if (string.IsNullOrWhiteSpace(referral.Service))
            {
                return BadRequest(
                    "Service is required."
                );
            }

            if (string.IsNullOrWhiteSpace(referral.Priority))
            {
                return BadRequest(
                    "Priority is required."
                );
            }

            // Only allow the three referral priority levels
            string[] validPriorities =
            {
                "P1",
                "P2",
                "P3"
            };

            if (!validPriorities.Contains(referral.Priority))
            {
                return BadRequest(
                    "Priority must be P1, P2 or P3."
                );
            }

            // New referrals always begin as pending
            referral.Status = "Pending";
            referral.StatusReason = null;
            referral.DateCreated = DateTime.Now;

            // Adds the referral to the database
            _context.Referrals.Add(referral);

            await _context.SaveChangesAsync();

            return Ok(referral);
        }

        // Update the priority and status of an existing referral
        [HttpPut("{referralId}")]
        public async Task<ActionResult<Referral>> UpdateReferral(
            int referralId,
            UpdateReferralRequest updatedReferral)
        {
            var referral =
                await _context.Referrals
                    .FirstOrDefaultAsync(
                        r => r.ReferralId == referralId
                    );

            if (referral == null)
            {
                return NotFound(
                    "Referral not found."
                );
            }

            // Only allow the three referral priority levels
            string[] validPriorities =
            {
                "P1",
                "P2",
                "P3"
            };

            if (string.IsNullOrWhiteSpace(
                    updatedReferral.Priority))
            {
                return BadRequest(
                    "Priority is required."
                );
            }

            if (!validPriorities.Contains(
                    updatedReferral.Priority))
            {
                return BadRequest(
                    "Priority must be P1, P2 or P3."
                );
            }

            // Only allow recognised referral statuses
            string[] validStatuses =
            {
                "Pending",
                "Accepted",
                "Returned for more information",
                "Rejected",
                "Completed"
            };

            if (string.IsNullOrWhiteSpace(
                    updatedReferral.Status))
            {
                return BadRequest(
                    "Status is required."
                );
            }

            if (!validStatuses.Contains(
                    updatedReferral.Status))
            {
                return BadRequest(
                    "Invalid referral status."
                );
            }

            // Returned and rejected referrals must include a reason
            if ((updatedReferral.Status ==
                    "Returned for more information" ||
                 updatedReferral.Status ==
                    "Rejected") &&
                string.IsNullOrWhiteSpace(
                    updatedReferral.StatusReason))
            {
                return BadRequest(
                    "A reason is required when a referral is returned or rejected."
                );
            }

            referral.Priority =
                updatedReferral.Priority;

            referral.Status =
                updatedReferral.Status;

            if (updatedReferral.Status ==
                    "Returned for more information" ||
                updatedReferral.Status ==
                    "Rejected")
            {
                referral.StatusReason =
                    updatedReferral.StatusReason!.Trim();
            }
            else
            {
                referral.StatusReason = null;
            }

            // Saves the referral changes to the database
            await _context.SaveChangesAsync();

            return Ok(referral);
        }
    }
}