using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReserveHealth.Api.Auth;
using ReserveHealth.Api.Data;
using ReserveHealth.Api.Models;
using ReserveHealth.Api.Services;

namespace ReserveHealth.Api.Controllers;

// Handles staff account signup and login.
// Authentication input is kept separate from the User database model.
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly ReserveHealthContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly TokenGenerate _tokenGenerate;

    public UserController(
        ReserveHealthContext context,
        IPasswordHasher<User> passwordHasher,
        TokenGenerate tokenGenerate)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenGenerate = tokenGenerate;
    }

    // Creates a new staff account and stores only a hashed password.
    [HttpPost("signup")]
    public async Task<IActionResult> Signup(SignupRequest request)
    {
        var email =
            request.Email
                .Trim()
                .ToLowerInvariant();

        var existingUser =
            await _context.Users
                .FirstOrDefaultAsync(
                    u => u.Email == email
                );

        if (existingUser != null)
        {
            return BadRequest(
                "An account with this email already exists"
            );
        }

        string[] validRoles =
        {
            "Doctor",
            "Lab Technician"
        };

        // Rejects roles that are not supported by the system.
        if (!validRoles.Contains(request.Role))
        {
            return BadRequest(
                "User must be Doctor or Lab Technician"
            );
        }

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = email,
            Role = request.Role
        };

        user.PasswordHash =
            _passwordHasher.HashPassword(
                user,
                request.Password
            );

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return Ok(
            "Account created successfully."
        );
    }

    // Verifies the supplied password against the stored password hash.
    // A signed JWT is returned only after authentication succeeds.
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var email =
            request.Email
                .Trim()
                .ToLowerInvariant();

        var user =
            await _context.Users
                .FirstOrDefaultAsync(
                    u => u.Email == email
                );

        if (user == null)
        {
            return Unauthorized(
                "Invalid email or password."
            );
        }

        var passwordResult =
            _passwordHasher
                .VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    request.Password
                );

        if (
            passwordResult ==
            PasswordVerificationResult.Failed
        )
        {
            return Unauthorized(
                "Invalid email or password."
            );
        }

        var token =
            _tokenGenerate.CreateToken(user);

        return Ok(
            new LoginResponse
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                Token = token
            }
        );
    }
}