using System.ComponentModel.DataAnnotations;

namespace ReserveHealth.Api.Auth;

// Stores the information required to create a staff account.
// Validation is performed on the backend so it cannot be bypassed
// by directly calling the API.
public class SignupRequest
{
    [Required]
    public string Name { get; set; } = "";

    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    [MinLength(8)]
    public string Password { get; set; } = "";
}