using System.ComponentModel.DataAnnotations;

namespace ReserveHealth.Api.Models;

public class TestResult
{
    public int TestResultId { get; set; }

    [Range(1, int.MaxValue)]
    public int TestRequestId { get; set; }

    public TestRequest TestRequest { get; set; } = null!;

    [Required]
    public string ResultInformation { get; set; } = "";

    [Range(1, int.MaxValue)]
    public int EnteredByUserId { get; set; }

    public User EnteredByUser { get; set; } = null!;

    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
}