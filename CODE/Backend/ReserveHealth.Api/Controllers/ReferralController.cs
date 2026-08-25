using Microsoft.AspNetCore.Mvc;
using ReserveHealth.Api.Models;

namespace ReserveHealth.Api.Controllers
{
    [ApiController]
    [Route("api/referrals")]
    public class ReferralController : ControllerBase
    {
        private static readonly List<Referral> Referrals = new();

        [HttpGet]
        public ActionResult<IEnumerable<Referral>> GetReferrals()
        {
            return Ok(Referrals);
        }

        [HttpPost]
        public ActionResult<Referral> CreateReferral(Referral referral)
        {
            if (string.IsNullOrWhiteSpace(referral.Reason))
            {
                return BadRequest("Reason is required.");
            }

            if (string.IsNullOrWhiteSpace(referral.Priority))
            {
                return BadRequest("Priority is required.");
            }

            referral.Status = "Pending";

            Referrals.Add(referral);

            return Ok(referral);
        }
    }
}