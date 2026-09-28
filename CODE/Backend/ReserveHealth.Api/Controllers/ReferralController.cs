using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
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

    [Authorize(Roles = "Doctor")]
    [ApiController]
    [Route("api/referrals")]
    public class ReferralController : ControllerBase
    {
        private readonly ReserveHealthContext _context;

        private static readonly string[] ValidPriorities =
        {
            "P1", "P2", "P3"
        };

        private static readonly Dictionary<string, string[]> AllowedTransitions =
            new()
            {
                ["Pending"] = new[]
                {
                    "Accepted",
                    "Returned for more information",
                    "Rejected"
                },
                ["Accepted"] = new[] { "Completed" },
                ["Returned for more information"] = new[] { "Pending" },
                ["Rejected"] = Array.Empty<string>(),
                ["Completed"] = Array.Empty<string>()
            };

        public ReferralController(ReserveHealthContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Referral>>> GetReferrals()
        {
            return Ok(await _context.Referrals.ToListAsync());
        }

        [HttpGet("{referralId}/history")]
        public async Task<IActionResult> GetReferralHistory(int referralId)
        {
            bool exists = await _context.Referrals
                .AnyAsync(r => r.ReferralId == referralId);

            if (!exists)
            {
                return NotFound("Referral not found.");
            }

            var history = await _context.ReferralStatusHistories
                .AsNoTracking()
                .Where(change => change.ReferralId == referralId)
                .OrderBy(change => change.ChangedAt)
                .ThenBy(change => change.ReferralStatusHistoryId)
                .ToListAsync();

            return Ok(history);
        }

        [HttpPost]
        public async Task<ActionResult<Referral>> CreateReferral(Referral referral)
        {
            if (referral.PatientId <= 0)
            {
                return BadRequest("Patient is required.");
            }

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

            if (!ValidPriorities.Contains(referral.Priority))
            {
                return BadRequest("Priority must be P1, P2 or P3.");
            }

            referral.Reason = referral.Reason.Trim();
            referral.Service = referral.Service.Trim();
            referral.Status = "Pending";
            referral.StatusReason = null;
            referral.DateCreated = DateTime.UtcNow;

            _context.Referrals.Add(referral);
            await _context.SaveChangesAsync();

            return Ok(referral);
        }

        [HttpPut("{referralId}")]
        public async Task<ActionResult<Referral>> UpdateReferral(
            int referralId,
            UpdateReferralRequest updatedReferral)
        {
            var referral = await _context.Referrals
                .FirstOrDefaultAsync(r => r.ReferralId == referralId);

            if (referral == null)
            {
                return NotFound("Referral not found.");
            }

            if (!ValidPriorities.Contains(updatedReferral.Priority))
            {
                return BadRequest("Priority must be P1, P2 or P3.");
            }

            if (!AllowedTransitions.ContainsKey(updatedReferral.Status))
            {
                return BadRequest("Invalid referral status.");
            }

            bool statusChanged = referral.Status != updatedReferral.Status;

            if (statusChanged &&
                (!AllowedTransitions.TryGetValue(
                    referral.Status, out var allowedStatuses) ||
                 !allowedStatuses.Contains(updatedReferral.Status)))
            {
                return BadRequest(
                    $"Cannot change referral status from {referral.Status} " +
                    $"to {updatedReferral.Status}.");
            }

            bool requiresReason =
                updatedReferral.Status == "Returned for more information" ||
                updatedReferral.Status == "Rejected";

            if (statusChanged &&
                requiresReason &&
                string.IsNullOrWhiteSpace(updatedReferral.StatusReason))
            {
                return BadRequest(
                    "A reason is required when a referral is returned or rejected.");
            }

            int userId;
            if (!int.TryParse(
                    User.FindFirstValue(ClaimTypes.NameIdentifier),
                    out userId))
            {
                return Unauthorized();
            }

            if (statusChanged)
            {
                _context.ReferralStatusHistories.Add(
                    new ReferralStatusHistory
                    {
                        ReferralId = referral.ReferralId,
                        PreviousStatus = referral.Status,
                        NewStatus = updatedReferral.Status,
                        Reason = requiresReason
                            ? updatedReferral.StatusReason!.Trim()
                            : null,
                        ChangedByUserId = userId,
                        ChangedAt = DateTime.UtcNow
                    });

                referral.Status = updatedReferral.Status;
                referral.StatusReason = requiresReason
                    ? updatedReferral.StatusReason!.Trim()
                    : null;
            }

            referral.Priority = updatedReferral.Priority;

            // The status and its history are saved together.
            await _context.SaveChangesAsync();

            return Ok(referral);
        }
    }
}