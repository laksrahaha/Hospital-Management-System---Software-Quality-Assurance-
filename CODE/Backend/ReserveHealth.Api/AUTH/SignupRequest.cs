using System.ComponentModel.DataAnnotations;

namespace ReserveHealth.Api.Auth;

// Stores the information required to create a staff account.
// Backend validation prevents invalid role values being submitted directly.
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

    [Required]
    public string Role { get; set; } = "";
}