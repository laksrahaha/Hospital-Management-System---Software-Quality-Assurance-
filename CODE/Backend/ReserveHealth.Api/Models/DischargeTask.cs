namespace ReserveHealth.Api.Models;

// This class stores the different tasks that need to be completed
// before or after a patient discharge.
// The tasks can be assigned to staff and tracked until they are completed.
public class DischargeTask
{
    public int DischargeTaskId { get; set; }

    public int DischargeId { get; set; }

    public int? AssignedToUserId { get; set; }

    public string TaskName { get; set; } = "";

    public string Description { get; set; } = "";

    public DateTime? DueDate { get; set; }

    public bool IsCompleted { get; set; } = false;

    public DateTime? CompletedDate { get; set; }

    public string Status { get; set; } = "Pending";

    public Discharge? Discharge { get; set; }

    public User? AssignedToUser { get; set; }
}