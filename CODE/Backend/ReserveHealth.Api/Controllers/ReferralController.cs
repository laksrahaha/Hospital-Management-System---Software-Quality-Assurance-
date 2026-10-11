using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReserveHealth.Api.Data;
using ReserveHealth.Api.Models;
using ReserveHealth.Api.Services;
using System.Security.Claims;

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
        private readonly AuditService _auditService;

        public ReferralController(
            ReserveHealthContext context,
            AuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Referral>>> GetReferrals()
        {
            var referrals =
                await _context.Referrals
                    .ToListAsync();

            return Ok(referrals);
        }

        [HttpPost]
        public async Task<ActionResult<Referral>> CreateReferral(
            Referral referral)
        {
            if (referral.PatientId <= 0)
            {
                return BadRequest(
                    "Patient is required."
                );
            }

            bool patientExists =
                await _context.Patients
                    .AnyAsync(
                        p => p.PatientId ==
                             referral.PatientId
                    );

            if (!patientExists)
            {
                return BadRequest(
                    "Selected patient does not exist."
                );
            }

            if (string.IsNullOrWhiteSpace(
                    referral.Reason))
            {
                return BadRequest(
                    "Reason is required."
                );
            }

            if (string.IsNullOrWhiteSpace(
                    referral.Service))
            {
                return BadRequest(
                    "Service is required."
                );
            }

            if (string.IsNullOrWhiteSpace(
                    referral.Priority))
            {
                return BadRequest(
                    "Priority is required."
                );
            }

            string[] validPriorities =
            {
                "P1",
                "P2",
                "P3"
            };

            if (!validPriorities.Contains(
                    referral.Priority))
            {
                return BadRequest(
                    "Priority must be P1, P2 or P3."
                );
            }

            referral.Status = "Pending";
            referral.StatusReason = null;
            referral.DateCreated = DateTime.Now;

            _context.Referrals.Add(referral);

            await _context.SaveChangesAsync();

            await _auditService.LogAsync(
                GetCurrentUserId(),
                "Referral Created",
                "Referral",
                referral.ReferralId,
                null,
                $"Priority: {referral.Priority}, Status: {referral.Status}"
            );

            return Ok(referral);
        }

        [HttpPut("{referralId}")]
        public async Task<ActionResult<Referral>> UpdateReferral(
            int referralId,
            UpdateReferralRequest updatedReferral)
        {
            var referral =
                await _context.Referrals
                    .FirstOrDefaultAsync(
                        r =>
                            r.ReferralId ==
                            referralId
                    );

            if (referral == null)
            {
                return NotFound(
                    "Referral not found."
                );
            }

            var oldPriority =
                referral.Priority;

            var oldStatus =
                referral.Status;

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

            if (
                (
                    updatedReferral.Status ==
                        "Returned for more information" ||
                    updatedReferral.Status ==
                        "Rejected"
                ) &&
                string.IsNullOrWhiteSpace(
                    updatedReferral.StatusReason)
            )
            {
                return BadRequest(
                    "A reason is required when a referral is returned or rejected."
                );
            }

            referral.Priority =
                updatedReferral.Priority;

            referral.Status =
                updatedReferral.Status;

            if (
                updatedReferral.Status ==
                    "Returned for more information" ||
                updatedReferral.Status ==
                    "Rejected"
            )
            {
                referral.StatusReason =
                    updatedReferral
                        .StatusReason!
                        .Trim();
            }
            else
            {
                referral.StatusReason = null;
            }

            await _context.SaveChangesAsync();

            if (oldPriority != referral.Priority)
            {
                await _auditService.LogAsync(
                    GetCurrentUserId(),
                    "Referral Priority Changed",
                    "Referral",
                    referral.ReferralId,
                    oldPriority,
                    referral.Priority
                );
            }

            if (oldStatus != referral.Status)
            {
                await _auditService.LogAsync(
                    GetCurrentUserId(),
                    "Referral Status Changed",
                    "Referral",
                    referral.ReferralId,
                    oldStatus,
                    referral.Status
                );
            }

            return Ok(referral);
        }

        private int GetCurrentUserId()
        {
            var userIdClaim =
                User.FindFirst(
                    ClaimTypes.NameIdentifier
                )?.Value
                ??
                User.FindFirst(
                    "userId"
                )?.Value
                ??
                User.FindFirst(
                    "sub"
                )?.Value;

            if (int.TryParse(
                    userIdClaim,
                    out var userId))
            {
                return userId;
            }

            throw new UnauthorizedAccessException(
                "Authenticated user ID could not be determined."
            );
        }
    }
}