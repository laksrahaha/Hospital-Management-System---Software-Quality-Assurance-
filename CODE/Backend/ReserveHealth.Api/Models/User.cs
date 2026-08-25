namespace ReserveHealth.Api.Models;

// This class stores the staff users that can access the system.
// The role is kept because different staff should eventually have
// different access to patient information.
public class User
{
    public int UserId { get; set; }

    public string Name { get; set; } = "";

    public string Email { get; set; } = "";

    public string PasswordHash { get; set; } = "";

    public string Role { get; set; } = "";

    public List<Discharge> DischargesInCharge { get; set; } = new();

    public List<MedicationCheck> VerifiedMedicationChecks { get; set; } = new();

    public List<DischargeTask> AssignedTasks { get; set; } = new();
}