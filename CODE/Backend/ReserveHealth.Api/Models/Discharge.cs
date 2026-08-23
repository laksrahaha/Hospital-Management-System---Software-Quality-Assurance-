namespace ReserveHealth.Api.Models;

// This class keeps the main discharge information for a patient.
// The doctor interview showed that the follow up plan and medication
// changes are important things that need to be clear before discharge.
public class Discharge
{
    public int DischargeId { get; set; }

    public int PatientId { get; set; }

    public int StaffInChargeId { get; set; }

    public DateTime PlannedDischargeDate { get; set; }

    public bool IsDischarged { get; set; } = false;

    public DateTime? ActualDischargeDate { get; set; }

    public string Status { get; set; } = "Pending";

    public string Diagnosis { get; set; } = "";

    public string FollowUpPlan { get; set; } = "";

    public string MedicationChanges { get; set; } = "";

    public string Notes { get; set; } = "";

    public Patient? Patient { get; set; }

    public User? StaffInCharge { get; set; }
}