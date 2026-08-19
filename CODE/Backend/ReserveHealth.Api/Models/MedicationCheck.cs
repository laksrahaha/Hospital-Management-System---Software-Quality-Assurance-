namespace ReserveHealth.Api.Models;

public class MedicationCheck
{
    public int MedicationCheckId { get; set; }

    public int DischargeId { get; set; }

    public string MedicationName { get; set; } = "";

    public bool IsVerified { get; set; } = false;

    public int? VerifiedByUserId { get; set; }

    public DateTime? VerifiedDate { get; set; }

    public string Notes { get; set; } = "";
}