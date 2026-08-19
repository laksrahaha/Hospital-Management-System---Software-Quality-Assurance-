namespace ReserveHealth.Api.Models;

public class Discharge
{
    public int DischargeId { get; set; }
    public int PatientId { get; set; }
    public int StaffinchargeID {get; set; }
    public DateTime PlannedDischargeDate { get; set; }
    public bool IsDischarged { get; set; } = false;
    public DateTime? ActualDischargeDate { get; set; }
    public string Status { get; set; } = "Pending";
    public string Notes { get; set; } = "";
}