namespace ReserveHealth.Api.Models;

public class ReferralStatusHistory
{
    public int ReferralStatusHistoryId { get; set; }

    public int ReferralId { get; set; }

    public string PreviousStatus { get; set; } = "";

    public string NewStatus { get; set; } = "";

    public string? Reason { get; set; }

    public int ChangedByUserId { get; set; }

    public DateTime ChangedAt { get; set; }
}