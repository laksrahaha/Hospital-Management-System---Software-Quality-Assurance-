using System.ComponentModel.DataAnnotations;

namespace ReserveHealth.Api.Models
{
    public class Referral
    {
        [Required(ErrorMessage = "Reason is required.")]
        public string Reason { get; set; } = string.Empty;

        [Required(ErrorMessage = "Priority is required.")]
        public string Priority { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending";
    }
}