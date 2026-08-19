using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.RegularExpressions;

namespace HospitalManagementSystem.Pages
{
    public class SignupModel : PageModel
    {
        [BindProperty]
        public string FullName { get; set; } = "";

        [BindProperty]
        public string Email { get; set; } = "";

        [BindProperty]
        public string Password { get; set; } = "";

        [BindProperty]
        public string ConfirmPassword { get; set; } = "";

        public void OnGet()
        {
        }

        public void OnPost()
        {
            // Full name validation
            if (string.IsNullOrWhiteSpace(FullName))
            {
                ModelState.AddModelError(
                    "FullName",
                    "Full name is required."
                );
            }

            // Email validation
            if (string.IsNullOrWhiteSpace(Email))
            {
                ModelState.AddModelError(
                    "Email",
                    "Email is required."
                );
            }

            // Password validation
            if (string.IsNullOrWhiteSpace(Password))
            {
                ModelState.AddModelError(
                    "Password",
                    "Password is required."
                );
            }
            else
            {
                bool validPassword =
                    Password.Length >= 8 &&
                    Regex.IsMatch(Password, "[A-Z]") &&
                    Regex.IsMatch(Password, "[0-9]") &&
                    Regex.IsMatch(Password, "[^a-zA-Z0-9]");

                if (!validPassword)
                {
                    ModelState.AddModelError(
                        "Password",
                        "Password does not meet the required format."
                    );
                }
            }

            // Confirm password validation
            if (string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                ModelState.AddModelError(
                    "ConfirmPassword",
                    "Please confirm your password."
                );
            }
            else if (Password != ConfirmPassword)
            {
                ModelState.AddModelError(
                    "ConfirmPassword",
                    "Passwords do not match."
                );
            }

            if (ModelState.IsValid)
            {
                // Account/database functionality can be added later.
            }
        }
    }
}