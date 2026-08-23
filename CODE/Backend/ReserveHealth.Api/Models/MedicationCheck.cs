namespace ReserveHealth.Api.Models;
// This class is used to check medication before the patient is discharged.
// It records whether the medication has been verified, who checked it
// and when it was checked.
public class MedicationCheck
{
    public int MedicationCheckId { get; set; }

    public int DischargeId { get; set; }

    public string MedicationName { get; set; } = "";

    public bool IsVerified { get; set; } = false;

    public int? VerifiedByUserId { get; set; }

    public DateTime? VerifiedDate { get; set; }

    public string Notes { get; set; } = "";

    public Discharge? Discharge { get; set; }
    
    public User? VerifiedByUser { get; set; }
}