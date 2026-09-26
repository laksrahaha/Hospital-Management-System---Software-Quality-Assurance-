namespace ReserveHealth.Api.Auth;

// Stores the safe user information returned after a successful login.
// Password information is never returned to the frontend.
public class LoginResponse
{
    public int UserId { get; set; }

    public string Name { get; set; } = "";

    public string Email { get; set; } = "";

    public string Role { get; set; } = "";
}