using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ReserveHealth.Tests
{
    [TestClass]
    public class AuthenticationTests
    {
        private WebApplicationFactory<Program> _factory = null!;


        [TestInitialize]
        public void Setup()
        {
            _factory =
                new WebApplicationFactory<Program>();
        }


        [TestCleanup]
        public void Cleanup()
        {
            _factory.Dispose();
        }


        [TestMethod]
        public async Task Login_InvalidEmail_ReturnsUnauthorized()
        {
            // Arrange
            var client =
                _factory.CreateClient();


            var loginRequest =
                new
                {
                    Email = "nonexistent.user@test.com",
                    Password = "Password123!"
                };


            // Act
            var response =
                await client.PostAsJsonAsync(
                    "/api/user/login",
                    loginRequest
                );


            // Assert
            Assert.AreEqual(
                HttpStatusCode.Unauthorized,
                response.StatusCode
            );
        }


        [TestMethod]
        public async Task Login_WrongPassword_ReturnsUnauthorized()
        {
            // Arrange
            var client =
                _factory.CreateClient();


            var uniqueEmail =
                $"wrongpasswordtest_{Guid.NewGuid():N}@test.com";


            var signupRequest =
                new
                {
                    Name = "Wrong Password Test User",
                    Email = uniqueEmail,
                    Password = "CorrectPassword123!",
                    Role = "Doctor"
                };


            var signupResponse =
                await client.PostAsJsonAsync(
                    "/api/user/signup",
                    signupRequest
                );


            Assert.IsTrue(
                signupResponse.IsSuccessStatusCode,
                "The test user must be created before testing an incorrect password."
            );


            var loginRequest =
                new
                {
                    Email = uniqueEmail,
                    Password = "DefinitelyWrong123!"
                };


            // Act
            var response =
                await client.PostAsJsonAsync(
                    "/api/user/login",
                    loginRequest
                );


            // Assert
            Assert.AreEqual(
                HttpStatusCode.Unauthorized,
                response.StatusCode
            );
        }


        [TestMethod]
        public async Task Signup_InvalidRole_ReturnsBadRequest()
        {
            // Arrange
            var client =
                _factory.CreateClient();


            var signupRequest =
                new
                {
                    Name = "Invalid Role Test User",
                    Email =
                        $"invalidrole_{Guid.NewGuid():N}@test.com",

                    Password = "Password123!",

                    Role = "Administrator"
                };


            // Act
            var response =
                await client.PostAsJsonAsync(
                    "/api/user/signup",
                    signupRequest
                );


            // Assert
            Assert.AreEqual(
                HttpStatusCode.BadRequest,
                response.StatusCode
            );
        }
    }
}