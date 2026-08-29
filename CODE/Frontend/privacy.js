// Gets the login form from the login page
const apiUrl = "http://localhost:5297/api/user";

const loginForm = document.getElementById("login-form"); 
 
if (loginForm) { 
 
    // Runs when the login form is submitted 
    loginForm.addEventListener("submit", async function (event) { 
 
        // Stops the page from refreshing 
        event.preventDefault(); 
 
        // Gets the login details entered by the user 
        const email = 
            document.getElementById("email").value.trim(); 
        const password = 
            document.getElementById("password").value; 
 
        // Gets the areas used to show error messages 
        const emailError = 
            document.getElementById("login-email-error"); 
        const passwordError = 
            document.getElementById("login-password-error"); 
 
        // Clears previous error messages 
        emailError.textContent = ""; 
        passwordError.textContent = ""; 
 
        let valid = true; 
 
        // Checks that an email was entered 
        if (email === "") { 
            emailError.textContent = 
                "Email is required."; 
            valid = false; 
        } 
 
        // Checks that a password was entered 
        if (password === "") { 
            passwordError.textContent = 
                "Password is required."; 
            valid = false; 
        } 
 
        // Opens the dashboard when the details are valid 
        if (valid) {

            try {

                const response = await fetch(
                    `${apiUrl}/login`,
                    {
                        method: "POST",
                        headers: {
                            "Content-Type": "application/json"
                        },
                        body: JSON.stringify({
                            email: email,
                            passwordHash: password
                        })
                    }
                );

                if (!response.ok) {
                    passwordError.textContent =
                        "Invalid email or password.";
                    return;
                }

                const user = await response.json();

                sessionStorage.setItem(
                    "reserveHealthUser",
                    JSON.stringify(user)
                );

                window.location.href =
                    "index.html";

            }

            catch (error) {

                console.error(error);

                passwordError.textContent =
                    "Could not connect to the server.";
            }
        } 
    }); 
} 
 
 
// Gets the sign up form from the sign up page 
const signupForm = 
    document.getElementById("signup-form"); 
 
if (signupForm) { 
 
    // Runs when the sign up form is submitted 
    signupForm.addEventListener("submit", async function (event) { 
 
        // Stops the page from refreshing 
        event.preventDefault(); 
 
        // Gets the account details entered by the user 
        const fullName = 
            document.getElementById("full-name").value.trim(); 
        const email = 
            document.getElementById("signup-email").value.trim(); 
        const password = 
            document.getElementById("signup-password").value; 
        const confirmPassword = 
            document.getElementById("confirm-password").value; 
 
        // Gets the areas used to show error messages 
        const fullNameError = 
            document.getElementById("full-name-error"); 
        const emailError = 
            document.getElementById("signup-email-error"); 
        const passwordError = 
            document.getElementById("signup-password-error"); 
        const confirmPasswordError = 
            document.getElementById("confirm-password-error"); 
 
        // Clears previous error messages 
        fullNameError.textContent = ""; 
        emailError.textContent = ""; 
        passwordError.textContent = ""; 
        confirmPasswordError.textContent = ""; 
 
        let valid = true; 
 
        // Checks that a full name was entered 
        if (fullName === "") { 
            fullNameError.textContent = 
                "Full name is required."; 
            valid = false; 
        } 
 
        // Checks that an email was entered 
        if (email === "") { 
            emailError.textContent = 
                "Email is required."; 
            valid = false; 
        } 
 
        // Checks that a password was entered 
        if (password === "") { 
            passwordError.textContent = 
                "Password is required."; 
            valid = false; 
        } 
        else { 
 
            // Checks that the password follows the required format 
            const validPassword = 
                password.length >= 8 && 
                /[A-Z]/.test(password) && 
                /[0-9]/.test(password) && 
                /[^a-zA-Z0-9]/.test(password); 
 
            if (!validPassword) { 
                passwordError.textContent = 
                    "Password does not meet the required format."; 
                valid = false; 
            } 
        } 
 
        // Checks that the password was confirmed 
        if (confirmPassword === "") { 
            confirmPasswordError.textContent = 
                "Please confirm your password."; 
            valid = false; 
        } 
        else if (password !== confirmPassword) { 
            confirmPasswordError.textContent = 
                "Passwords do not match."; 
            valid = false; 
        } 
 
        // Confirms the account and returns to the login page 
        if (valid) {

            try {

                const response = await fetch(
                    `${apiUrl}/signup`,
                    {
                        method: "POST",
                        headers: {
                            "Content-Type": "application/json"
                        },
                        body: JSON.stringify({
                            name: fullName,
                            email: email,
                            passwordHash: password,
                            role: "Doctor"
                        })
                    }
                );

                if (!response.ok) {

                    const message =
                        await response.text();

                    emailError.textContent =
                        message;

                    return;
                }

                alert("Account created successfully.");

                window.location.href =
                    "login.html";

            }

            catch (error) {

                console.error(error);

                emailError.textContent =
                    "Could not connect to the server.";
            }
        } 
    }); 
}