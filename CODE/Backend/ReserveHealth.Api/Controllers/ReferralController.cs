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
    [ApiController]
    [Route("api/referrals")]
    public class ReferralController : ControllerBase
    {
        private static readonly List<Referral> Referrals = new();

        private readonly ReserveHealthContext _context;

        public ReferralController(ReserveHealthContext context)
        {
            _context = context;
        }

        // Get all referrals
        [HttpGet]
        public ActionResult<IEnumerable<Referral>> GetReferrals()
        {
            return Ok(Referrals);
        }

        // Create a new referral
        [HttpPost]
        public async Task<ActionResult<Referral>> CreateReferral(Referral referral)
        {
            if (referral.PatientId <= 0)
            {
                return BadRequest("Patient is required.");
            }

            // Check that the selected patient actually exists
            bool patientExists = await _context.Patients
                .AnyAsync(p => p.PatientId == referral.PatientId);

            if (!patientExists)
            {
                return BadRequest("Selected patient does not exist.");
            }

            if (string.IsNullOrWhiteSpace(referral.Reason))
            {
                return BadRequest("Reason is required.");
            }

            if (string.IsNullOrWhiteSpace(referral.Service))
            {
                return BadRequest("Service is required.");
            }

            if (string.IsNullOrWhiteSpace(referral.Priority))
            {
                return BadRequest("Priority is required.");
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
                return BadRequest("Priority must be P1, P2 or P3.");
            }

            // Generate the next referral ID
            referral.ReferralId = Referrals.Count == 0
                ? 1
                : Referrals.Max(r => r.ReferralId) + 1;

            referral.Status = "Pending";
            referral.StatusReason = null;
            referral.DateCreated = DateTime.Now;

            Referrals.Add(referral);

            return Ok(referral);
        }

        // Update the status of an existing referral
        [HttpPut("{referralId}/status")]
        public ActionResult<Referral> UpdateReferralStatus(
            int referralId,
            Referral updatedReferral)
        {
            var referral = Referrals.FirstOrDefault(
                r => r.ReferralId == referralId);

            if (referral == null)
            {
                return NotFound("Referral not found.");
            }

            string[] validStatuses =
            {
                "Pending",
                "Accepted",
                "Returned for more information",
                "Rejected",
                "Completed"
            };

            if (!validStatuses.Contains(updatedReferral.Status))
            {
                return BadRequest("Invalid referral status.");
            }

            // Returned and rejected referrals need a reason
            if ((updatedReferral.Status == "Returned for more information" ||
                 updatedReferral.Status == "Rejected") &&
                string.IsNullOrWhiteSpace(updatedReferral.StatusReason))
            {
                return BadRequest(
                    "A reason is required when a referral is returned or rejected.");
            }

            referral.Status = updatedReferral.Status;

            if (updatedReferral.Status == "Returned for more information" ||
                updatedReferral.Status == "Rejected")
            {
                referral.StatusReason = updatedReferral.StatusReason;
            }
            else
            {
                referral.StatusReason = null;
            }

            return Ok(referral);
        }
    }
}