using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ReserveHealth.Tests
{
    [TestClass]
    public class PatientReferralIntegrationTests
    {
        private ReserveHealthWebApplicationFactory _factory = null!;
        private HttpClient _client = null!;

        [TestInitialize]
        public void Setup()
        {
            _factory = new ReserveHealthWebApplicationFactory();

            var authenticatedFactory = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = "Test";
                        options.DefaultChallengeScheme = "Test";
                        options.DefaultForbidScheme = "Test";
                    })
                    .AddScheme<
                        AuthenticationSchemeOptions,
                        TestAuthenticationHandler>(
                        "Test",
                        options => { });
                });
            });

            _client = authenticatedFactory.CreateClient();
            _client.DefaultRequestHeaders.Add("X-Test-Role", "Doctor");
        }

        [TestCleanup]
        public void Cleanup()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [TestMethod]
        public async Task CreateReferral_ValidPatient_SavesCorrectPatientId()
        {
            int patientId = await GetExistingPatientId();
            var before = await GetReferrals();

            var created = await CreateReferral(patientId);
            int referralId = created.GetProperty("referralId").GetInt32();

            Assert.AreEqual(
                patientId,
                created.GetProperty("patientId").GetInt32());

            Assert.AreEqual(
                "Pending",
                created.GetProperty("status").GetString());

            var after = await GetReferrals();

            Assert.AreEqual(before.Length + 1, after.Length);

            var saved = after.Single(referral =>
                referral.GetProperty("referralId").GetInt32() == referralId);

            Assert.AreEqual(
                patientId,
                saved.GetProperty("patientId").GetInt32());

            Assert.AreEqual(
                "Cardiology",
                saved.GetProperty("service").GetString());

            Assert.AreEqual(
                "P2",
                saved.GetProperty("priority").GetString());

            Assert.AreEqual(
                "Pending",
                saved.GetProperty("status").GetString());
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(int.MaxValue)]
        public async Task CreateReferral_InvalidPatient_DoesNotCreateReferral(
            int patientId)
        {
            // Confirm the positive test ID does not belong to a patient.
            if (patientId > 0)
            {
                using var patientResponse =
                    await _client.GetAsync($"/api/patient/{patientId}");

                Assert.AreEqual(
                    HttpStatusCode.NotFound,
                    patientResponse.StatusCode);
            }

            var before = await GetReferrals();

            using var response = await _client.PostAsJsonAsync(
                "/api/referrals",
                new
                {
                    PatientId = patientId,
                    Service = "Cardiology",
                    Reason = "Invalid patient integration test",
                    Priority = "P2"
                });

            Assert.AreEqual(
                HttpStatusCode.BadRequest,
                response.StatusCode);

            var after = await GetReferrals();

            // Compare every referral, not just the total count.
            var beforeRecords = before
                .OrderBy(referral =>
                    referral.GetProperty("referralId").GetInt32())
                .Select(referral => referral.GetRawText())
                .ToArray();

            var afterRecords = after
                .OrderBy(referral =>
                    referral.GetProperty("referralId").GetInt32())
                .Select(referral => referral.GetRawText())
                .ToArray();

            CollectionAssert.AreEqual(
                beforeRecords,
                afterRecords,
                "A rejected request must leave referral records unchanged.");
        }

        [TestMethod]
        public async Task ArchivePatient_KeepsReferralHistoryAndPatientId()
        {
            int patientId = await GetExistingPatientId();

            var created = await CreateReferral(patientId);
            int referralId = created.GetProperty("referralId").GetInt32();

            var before = await GetReferrals();

            using var archiveResponse = await _client.PutAsync(
                $"/api/patient/{patientId}/archive",
                null);

            Assert.AreEqual(
                HttpStatusCode.NoContent,
                archiveResponse.StatusCode);

            // Archived patients must still be accessible by their ID.
            using var patientResponse = await _client.GetAsync(
                $"/api/patient/{patientId}");

            Assert.AreEqual(
                HttpStatusCode.OK,
                patientResponse.StatusCode);

            using var patientDocument = JsonDocument.Parse(
                await patientResponse.Content.ReadAsStringAsync());

            var patient = patientDocument.RootElement;

            Assert.AreEqual(
                patientId,
                patient.GetProperty("patientId").GetInt32());

            Assert.IsFalse(
                patient.GetProperty("isActive").GetBoolean());

            // Confirm the patient appears in the archived patient list.
            using var archivedResponse = await _client.GetAsync(
                "/api/patient/archived");

            archivedResponse.EnsureSuccessStatusCode();

            using var archivedDocument = JsonDocument.Parse(
                await archivedResponse.Content.ReadAsStringAsync());

            Assert.IsTrue(
                archivedDocument.RootElement
                    .EnumerateArray()
                    .Any(item =>
                        item.GetProperty("patientId").GetInt32() == patientId));

            // The existing referral API must still return the same records.
            var after = await GetReferrals();

            Assert.AreEqual(before.Length, after.Length);

            var beforePatientReferrals = before
                .Where(referral =>
                    referral.GetProperty("patientId").GetInt32() == patientId)
                .OrderBy(referral =>
                    referral.GetProperty("referralId").GetInt32())
                .Select(referral => referral.GetRawText())
                .ToArray();

            var afterPatientReferrals = after
                .Where(referral =>
                    referral.GetProperty("patientId").GetInt32() == patientId)
                .OrderBy(referral =>
                    referral.GetProperty("referralId").GetInt32())
                .Select(referral => referral.GetRawText())
                .ToArray();

            CollectionAssert.AreEqual(
                beforePatientReferrals,
                afterPatientReferrals,
                "Archiving must preserve the patient's existing referrals.");

            var retainedReferral = after.Single(referral =>
                referral.GetProperty("referralId").GetInt32() == referralId);

            Assert.AreEqual(
                patientId,
                retainedReferral.GetProperty("patientId").GetInt32());
        }

        private async Task<int> GetExistingPatientId()
        {
            using var response = await _client.GetAsync("/api/patient");
            response.EnsureSuccessStatusCode();

            using var document = JsonDocument.Parse(
                await response.Content.ReadAsStringAsync());

            var patients = document.RootElement;

            Assert.IsTrue(
                patients.GetArrayLength() > 0,
                "The test database must contain an active patient.");

            return patients[0].GetProperty("patientId").GetInt32();
        }

        private async Task<JsonElement> CreateReferral(int patientId)
        {
            using var response = await _client.PostAsJsonAsync(
                "/api/referrals",
                new
                {
                    PatientId = patientId,
                    Service = "Cardiology",
                    Reason = "Patient referral integration test",
                    Priority = "P2"
                });

            Assert.AreEqual(
                HttpStatusCode.OK,
                response.StatusCode,
                await response.Content.ReadAsStringAsync());

            using var document = JsonDocument.Parse(
                await response.Content.ReadAsStringAsync());

            return document.RootElement.Clone();
        }

        private async Task<JsonElement[]> GetReferrals()
        {
            using var response = await _client.GetAsync("/api/referrals");
            response.EnsureSuccessStatusCode();

            using var document = JsonDocument.Parse(
                await response.Content.ReadAsStringAsync());

            return document.RootElement
                .EnumerateArray()
                .Select(referral => referral.Clone())
                .ToArray();
        }
    }
}