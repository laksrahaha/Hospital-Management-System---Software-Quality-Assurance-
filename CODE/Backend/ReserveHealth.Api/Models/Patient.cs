namespace ReserveHealth.Api.Models;

public class Patient
{
    public int PatientId { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public DateTime DateOfBirth { get; set; }
    public string Status { get; set; } = "Admitted";
}