namespace ReserveHealth.Api.Models;

// This class stores the main patient information.
// Medical history, allergies and location are included because
// they are useful when staff are viewing a patient's record.
public class Patient
{
    public int PatientId { get; set; }

    public string FirstName { get; set; } = "";

    public string LastName { get; set; } = "";

    public DateTime DateOfBirth { get; set; }

    public string Status { get; set; } = "Admitted";

    public string Location { get; set; } = "";

    public string MedicalHistorySummary { get; set; } = "";

    public string Allergies { get; set; } = "";

    public bool IsActive { get; set; } = true;

    public List<Discharge> Discharges { get; set; } = new();
}