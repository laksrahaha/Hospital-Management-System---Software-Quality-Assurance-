using System.ComponentModel.DataAnnotations;

namespace ReserveHealth.Api.Auth;

// Stores only the information required when a staff member logs in.
// This keeps login data separate from the full User database model.
public class LoginRequest
{
    [Required]// attributes telling asp.net how to work with the data
    [EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";
}