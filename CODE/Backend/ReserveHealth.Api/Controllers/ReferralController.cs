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

        private static readonly string[] ValidPriorities =
        {
            "P1",
            "P2",
            "P3"
        };

        private static readonly Dictionary<string, string[]>
            AllowedTransitions =
                new()
                {
                    ["Pending"] = new[]
                    {
                        "Accepted",
                        "Returned for more information",
                        "Rejected"
                    },

                    ["Accepted"] = new[]
                    {
                        "Completed"
                    },

                    ["Returned for more information"] = new[]
                    {
                        "Pending"
                    },

                    ["Rejected"] =
                        Array.Empty<string>(),

                    ["Completed"] =
                        Array.Empty<string>()
                };

        public ReferralController(
            ReserveHealthContext context,
            AuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        // Get all referrals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Referral>>>
            GetReferrals()
        {
            var referrals =
                await _context.Referrals
                    .ToListAsync();

            return Ok(referrals);
        }

        // Get the status history for one referral
        [HttpGet("{referralId}/history")]
        public async Task<IActionResult>
            GetReferralHistory(int referralId)
        {
            bool exists =
                await _context.Referrals
                    .AnyAsync(
                        r =>
                            r.ReferralId ==
                            referralId
                    );

            if (!exists)
            {
                return NotFound(
                    "Referral not found."
                );
            }

            var history =
                await _context
                    .ReferralStatusHistories
                    .AsNoTracking()
                    .Where(
                        change =>
                            change.ReferralId ==
                            referralId
                    )
                    .OrderBy(
                        change =>
                            change.ChangedAt
                    )
                    .ThenBy(
                        change =>
                            change
                                .ReferralStatusHistoryId
                    )
                    .ToListAsync();

            return Ok(history);
        }

        // Create a new referral
        [HttpPost]
        public async Task<ActionResult<Referral>>
            CreateReferral(
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
                        p =>
                            p.PatientId ==
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

            if (!ValidPriorities.Contains(
                    referral.Priority))
            {
                return BadRequest(
                    "Priority must be P1, P2 or P3."
                );
            }

            referral.Reason =
                referral.Reason.Trim();

            referral.Service =
                referral.Service.Trim();

            // New referrals always start as Pending.
            referral.Status = "Pending";
            referral.StatusReason = null;
            referral.DateCreated =
                DateTime.UtcNow;

            _context.Referrals.Add(
                referral
            );

            await _context.SaveChangesAsync();

            // Record the successful creation in the audit trail.
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

        // Update an existing referral.
        [HttpPut("{referralId}")]
        public async Task<ActionResult<Referral>>
            UpdateReferral(
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

            // Keep the previous values for history and audit logging.
            var oldPriority =
                referral.Priority;

            var oldStatus =
                referral.Status;

            if (string.IsNullOrWhiteSpace(
                    updatedReferral.Priority))
            {
                return BadRequest(
                    "Priority is required."
                );
            }

            if (!ValidPriorities.Contains(
                    updatedReferral.Priority))
            {
                return BadRequest(
                    "Priority must be P1, P2 or P3."
                );
            }

            if (string.IsNullOrWhiteSpace(
                    updatedReferral.Status))
            {
                return BadRequest(
                    "Status is required."
                );
            }

            if (!AllowedTransitions.ContainsKey(
                    updatedReferral.Status))
            {
                return BadRequest(
                    "Invalid referral status."
                );
            }

            bool statusChanged =
                referral.Status !=
                updatedReferral.Status;

            // Prevent invalid workflow transitions.
            if (
                statusChanged &&
                (
                    !AllowedTransitions.TryGetValue(
                        referral.Status,
                        out var allowedStatuses
                    ) ||
                    !allowedStatuses.Contains(
                        updatedReferral.Status
                    )
                )
            )
            {
                return BadRequest(
                    $"Cannot change referral status from {referral.Status} " +
                    $"to {updatedReferral.Status}."
                );
            }

            bool requiresReason =
                updatedReferral.Status ==
                    "Returned for more information" ||
                updatedReferral.Status ==
                    "Rejected";

            if (
                statusChanged &&
                requiresReason &&
                string.IsNullOrWhiteSpace(
                    updatedReferral.StatusReason)
            )
            {
                return BadRequest(
                    "A reason is required when a referral is returned or rejected."
                );
            }

            var userId =
                GetCurrentUserId();

            // Add status history only when the status really changes.
            if (statusChanged)
            {
                _context
                    .ReferralStatusHistories
                    .Add(
                        new ReferralStatusHistory
                        {
                            ReferralId =
                                referral.ReferralId,

                            PreviousStatus =
                                referral.Status,

                            NewStatus =
                                updatedReferral.Status,

                            Reason =
                                requiresReason
                                    ? updatedReferral
                                        .StatusReason!
                                        .Trim()
                                    : null,

                            ChangedByUserId =
                                userId,

                            ChangedAt =
                                DateTime.UtcNow
                        }
                    );

                referral.Status =
                    updatedReferral.Status;

                referral.StatusReason =
                    requiresReason
                        ? updatedReferral
                            .StatusReason!
                            .Trim()
                        : null;
            }

            referral.Priority =
                updatedReferral.Priority;

            // Referral and status history are saved together.
            await _context.SaveChangesAsync();

            // Record priority changes in audit trail.
            if (oldPriority != referral.Priority)
            {
                await _auditService.LogAsync(
                    userId,
                    "Referral Priority Changed",
                    "Referral",
                    referral.ReferralId,
                    oldPriority,
                    referral.Priority
                );
            }

            // Record status changes in audit trail.
            if (oldStatus != referral.Status)
            {
                await _auditService.LogAsync(
                    userId,
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