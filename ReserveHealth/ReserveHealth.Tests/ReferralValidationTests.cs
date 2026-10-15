using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ReserveHealth.Tests
{
    [TestClass]
    public class ReferralValidationTests
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
        public async Task CreateReferral_MissingPatient_ReturnsBadRequest()
        {
            // Arrange
            var client =
                CreateDoctorClient();


            var referral =
                new
                {
                    PatientId = 0,
                    Reason = "Chest pain",
                    Service = "Cardiology",
                    Priority = "P1"
                };


            // Act
            var response =
                await client.PostAsJsonAsync(
                    "/api/referrals",
                    referral
                );


            // Assert
            Assert.AreEqual(
                HttpStatusCode.BadRequest,
                response.StatusCode
            );
        }


        [TestMethod]
        public async Task CreateReferral_BlankReason_ReturnsBadRequest()
        {
            // Arrange
            var client =
                CreateDoctorClient();


            var patientId =
                await GetExistingPatientId(
                    client
                );


            var referral =
                new
                {
                    PatientId = patientId,
                    Reason = "   ",
                    Service = "Cardiology",
                    Priority = "P1"
                };


            // Act
            var response =
                await client.PostAsJsonAsync(
                    "/api/referrals",
                    referral
                );


            // Assert
            Assert.AreEqual(
                HttpStatusCode.BadRequest,
                response.StatusCode
            );
        }


        [TestMethod]
        public async Task CreateReferral_InvalidPriority_ReturnsBadRequest()
        {
            // Arrange
            var client =
                CreateDoctorClient();


            var patientId =
                await GetExistingPatientId(
                    client
                );


            var referral =
                new
                {
                    PatientId = patientId,
                    Reason = "Chest pain",
                    Service = "Cardiology",
                    Priority = "P4"
                };


            // Act
            var response =
                await client.PostAsJsonAsync(
                    "/api/referrals",
                    referral
                );


            // Assert
            Assert.AreEqual(
                HttpStatusCode.BadRequest,
                response.StatusCode
            );
        }


        [TestMethod]
        public async Task UpdateReferral_InvalidStatusTransition_ReturnsBadRequest()
        {
            // Arrange
            var client =
                CreateDoctorClient();


            var patientId =
                await GetExistingPatientId(
                    client
                );


            var newReferral =
                new
                {
                    PatientId = patientId,
                    Reason = "Automated transition test",
                    Service = "Cardiology",
                    Priority = "P1"
                };


            var createResponse =
                await client.PostAsJsonAsync(
                    "/api/referrals",
                    newReferral
                );


            Assert.IsTrue(
                createResponse.IsSuccessStatusCode,
                "A referral must be created before testing the status transition."
            );


            var json =
                await createResponse.Content
                    .ReadAsStringAsync();


            using var document =
                JsonDocument.Parse(
                    json
                );


            var referralId =
                document.RootElement
                    .GetProperty(
                        "referralId"
                    )
                    .GetInt32();


            var updateRequest =
                new
                {
                    Priority = "P1",
                    Status = "Completed",
                    StatusReason = (string?)null
                };


            // Act
            var response =
                await client.PutAsJsonAsync(
                    $"/api/referrals/{referralId}",
                    updateRequest
                );


            // Assert
            Assert.AreEqual(
                HttpStatusCode.BadRequest,
                response.StatusCode
            );
        }


        private async Task<int> GetExistingPatientId(
            HttpClient client)
        {
            var patientResponse =
                await client.GetAsync(
                    "/api/patient"
                );


            Assert.IsTrue(
                patientResponse.IsSuccessStatusCode
            );


            var json =
                await patientResponse.Content
                    .ReadAsStringAsync();


            using var document =
                JsonDocument.Parse(
                    json
                );


            var patients =
                document.RootElement;


            Assert.IsTrue(
                patients.GetArrayLength() > 0,
                "At least one active patient is required for this test."
            );


            return patients[0]
                .GetProperty(
                    "patientId"
                )
                .GetInt32();
        }


        private HttpClient CreateDoctorClient()
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
                "Doctor"
            );


            return client;
        }
    }
}