// Referral model
// Stores the information needed for a patient referral
// including the patient, service, priority and referral status.

using System.ComponentModel.DataAnnotations;

namespace ReserveHealth.Api.Models
{
    public class Referral
    {
        public int ReferralId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Patient is required.")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Reason is required.")]
        public string Reason { get; set; } = string.Empty;

        [Required(ErrorMessage = "Service is required.")]
        public string Service { get; set; } = string.Empty;

        [Required(ErrorMessage = "Priority is required.")]
        public string Priority { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending";

        public string? StatusReason { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}