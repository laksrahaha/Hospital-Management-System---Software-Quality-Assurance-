namespace ReserveHealth.Api.Models;

// This class stores the main patient information.
// I added the medical history and allergies because these are some of the
// first things the doctor would need to see when opening a patient record.
public class Patient
{
    public int PatientId { get; set; }

    public string FirstName { get; set; } = "";

    public string LastName { get; set; } = "";

    public DateTime DateOfBirth { get; set; }

    public string Status { get; set; } = "Admitted";

    public string MedicalHistorySummary { get; set; } = "";

    public string Allergies { get; set; } = "";

    public List<Discharge> Discharges { get; set; } = new();
}