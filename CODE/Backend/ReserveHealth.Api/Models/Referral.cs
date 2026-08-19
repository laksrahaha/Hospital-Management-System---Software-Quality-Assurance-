namespace ReserveHealth.Api.Models;

public class Referral
{
    public int ReferralId { get; set; }

    public int DischargeId { get; set; }

    public string ReferralType { get; set; } = "";

    public string Organisation { get; set; } = "";

    public string Status { get; set; } = "Pending";

    public DateTime? ReferralDate { get; set; }

    public DateTime? FollowUpDate { get; set; }

    public string Notes { get; set; } = "";

}