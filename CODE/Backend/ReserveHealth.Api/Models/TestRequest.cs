using System.ComponentModel.DataAnnotations;

namespace ReserveHealth.Api.Models;

public class TestRequest
{
    public int TestRequestId { get; set; }

    [Range(1, int.MaxValue)]
    public int PatientId { get; set; }

    public Patient Patient { get; set; } = null!;

    [Required]
    public string TestType { get; set; } = "";

    public string Status { get; set; } = "Outstanding";

    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
}