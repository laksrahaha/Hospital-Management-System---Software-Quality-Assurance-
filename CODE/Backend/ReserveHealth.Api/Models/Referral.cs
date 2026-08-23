namespace ReserveHealth.Api.Models;

// This class stores referrals linked to the patient discharge process.
// The referral can have a priority and status because referrals can be
// triaged, rejected, accepted or sent back for more information.
public class Referral
{
    public int ReferralId { get; set; }

    public int DischargeId { get; set; }

    public string ReferralType { get; set; } = "";

    public string Organisation { get; set; } = "";

    public string Reason { get; set; } = "";

    public string Priority { get; set; } = "";

    public string Status { get; set; } = "Pending";

    public DateTime? ReferralDate { get; set; }

    public DateTime? FollowUpDate { get; set; }

    public string Notes { get; set; } = "";

    public Discharge? Discharge { get; set; }
}