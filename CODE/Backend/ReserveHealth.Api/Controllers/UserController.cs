using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReserveHealth.Api.Data;
using ReserveHealth.Api.Models;

namespace ReserveHealth.Api.Controllers;

// Handles staff account signup and login.
// This connects the frontend login pages to the user records in the database.
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly ReserveHealthContext _context;

    public UserController(ReserveHealthContext context)
    {
        _context = context;
    }

    [HttpPost("signup")]
    public async Task<IActionResult> Signup(User user)
    {
        var existingUser =
            await _context.Users
                .FirstOrDefaultAsync(u => u.Email == user.Email);

        if (existingUser != null)
        {
            return BadRequest("An account with this email already exists.");
        }

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(User loginUser)
    {
        var user =
            await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginUser.Email);

        if (user == null)
        {
            return Unauthorized("Invalid email or password.");
        }

        if (user.PasswordHash != loginUser.PasswordHash)
        {
            return Unauthorized("Invalid email or password.");
        }

        return Ok(new
        {
            user.UserId,
            user.Name,
            user.Email,
            user.Role
        });
    }
}