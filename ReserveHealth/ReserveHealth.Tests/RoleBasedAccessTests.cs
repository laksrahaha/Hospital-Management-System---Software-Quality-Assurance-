using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ReserveHealth.Tests
{
    [TestClass]
    public class RoleBasedAccessTests
    {
        private ReserveHealthWebApplicationFactory _factory = null!;


        [TestInitialize]
        public void Setup()
        {
            _factory =
                new ReserveHealthWebApplicationFactory();
        }


        [TestCleanup]
        public void Cleanup()
        {
            _factory.Dispose();
        }


        [TestMethod]
        public async Task PatientApi_NoAuthentication_ReturnsUnauthorized()
        {
            // Arrange
            var client =
                _factory.CreateClient();


            // Act
            var response =
                await client.GetAsync(
                    "/api/patient"
                );


            // Assert
            Assert.AreEqual(
                HttpStatusCode.Unauthorized,
                response.StatusCode
            );
        }


        [TestMethod]
        public async Task PatientApi_LabTechnician_ReturnsForbidden()
        {
            // Arrange
            var client =
                CreateAuthenticatedClient(
                    "Lab Technician"
                );


            // Act
            var response =
                await client.GetAsync(
                    "/api/patient"
                );


            // Assert
            Assert.AreEqual(
                HttpStatusCode.Forbidden,
                response.StatusCode
            );
        }


        [TestMethod]
        public async Task PatientApi_Doctor_ReturnsSuccess()
        {
            // Arrange
            var client =
                CreateAuthenticatedClient(
                    "Doctor"
                );


            // Act
            var response =
                await client.GetAsync(
                    "/api/patient"
                );


            // Assert
            Assert.IsTrue(
                response.IsSuccessStatusCode
            );
        }


        [TestMethod]
        public async Task TestRequestsApi_Doctor_ReturnsForbidden()
        {
            // Arrange
            var client =
                CreateAuthenticatedClient(
                    "Doctor"
                );


            // Act
            var response =
                await client.GetAsync(
                    "/api/test-requests"
                );


            // Assert
            Assert.AreEqual(
                HttpStatusCode.Forbidden,
                response.StatusCode
            );
        }


        [TestMethod]
        public async Task TestRequestsApi_LabTechnician_ReturnsSuccess()
        {
            // Arrange
            var client =
                CreateAuthenticatedClient(
                    "Lab Technician"
                );


            // Act
            var response =
                await client.GetAsync(
                    "/api/test-requests"
                );


            // Assert
            Assert.IsTrue(
                response.IsSuccessStatusCode
            );
        }


        [TestMethod]
        public async Task ReferralsApi_LabTechnician_ReturnsForbidden()
        {
            // Arrange
            var client =
                CreateAuthenticatedClient(
                    "Lab Technician"
                );


            // Act
            var response =
                await client.GetAsync(
                    "/api/referrals"
                );


            // Assert
            Assert.AreEqual(
                HttpStatusCode.Forbidden,
                response.StatusCode
            );
        }


        [TestMethod]
        public async Task ReferralsApi_Doctor_ReturnsSuccess()
        {
            // Arrange
            var client =
                CreateAuthenticatedClient(
                    "Doctor"
                );


            // Act
            var response =
                await client.GetAsync(
                    "/api/referrals"
                );


            // Assert
            Assert.IsTrue(
                response.IsSuccessStatusCode
            );
        }


        [TestMethod]
        public async Task AuditApi_LabTechnician_ReturnsForbidden()
        {
            // Arrange
            var client =
                CreateAuthenticatedClient(
                    "Lab Technician"
                );


            // Act
            var response =
                await client.GetAsync(
                    "/api/audit"
                );


            // Assert
            Assert.AreEqual(
                HttpStatusCode.Forbidden,
                response.StatusCode
            );
        }


        [TestMethod]
        public async Task AuditApi_Doctor_ReturnsSuccess()
        {
            // Arrange
            var client =
                CreateAuthenticatedClient(
                    "Doctor"
                );


            // Act
            var response =
                await client.GetAsync(
                    "/api/audit"
                );


            // Assert
            Assert.IsTrue(
                response.IsSuccessStatusCode
            );
        }


        private HttpClient CreateAuthenticatedClient(
            string role)
        {
            var authenticatedFactory =
                _factory.WithWebHostBuilder(
                    builder =>
                    {
                        builder.ConfigureTestServices(
                            services =>
                            {
                                services
                                    .AddAuthentication(
                                        options =>
                                        {
                                            options.DefaultAuthenticateScheme =
                                                "Test";

                                            options.DefaultChallengeScheme =
                                                "Test";

                                            options.DefaultForbidScheme =
                                                "Test";
                                        }
                                    )
                                    .AddScheme<
                                        AuthenticationSchemeOptions,
                                        TestAuthenticationHandler
                                    >(
                                        "Test",
                                        options =>
                                        {
                                        }
                                    );
                            }
                        );
                    }
                );


            var client =
                authenticatedFactory.CreateClient();


            client.DefaultRequestHeaders.Add(
                "X-Test-Role",
                role
            );


            return client;
        }
    }


    public class TestAuthenticationHandler
        : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(
                options,
                logger,
                encoder)
        {
        }


        protected override Task<AuthenticateResult>
            HandleAuthenticateAsync()
        {
            if (
                !Request.Headers.TryGetValue(
                    "X-Test-Role",
                    out var roleHeader)
            )
            {
                return Task.FromResult(
                    AuthenticateResult.Fail(
                        "Test role was not supplied."
                    )
                );
            }


            var role =
                roleHeader.ToString();


            var claims =
                new[]
                {
                    new Claim(
                        ClaimTypes.NameIdentifier,
                        "999"
                    ),

                    new Claim(
                        ClaimTypes.Name,
                        "Automated Test User"
                    ),

                    new Claim(
                        ClaimTypes.Role,
                        role
                    )
                };


            var identity =
                new ClaimsIdentity(
                    claims,
                    "Test"
                );


            var principal =
                new ClaimsPrincipal(
                    identity
                );


            var ticket =
                new AuthenticationTicket(
                    principal,
                    "Test"
                );


            return Task.FromResult(
                AuthenticateResult.Success(
                    ticket
                )
            );
        }
    }
}