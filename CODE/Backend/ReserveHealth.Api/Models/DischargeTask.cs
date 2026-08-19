namespace ReserveHealth.Api.Models;

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
}