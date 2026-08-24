using ReserveHealth.Api.Models;

namespace ReserveHealth.Api.Data;

// This adds sample patient records so we have data
// to use while testing and showing the prototype.
// makes it easier to dmeonstrate workflow and fucntionality without having to manually add patients each time.
public static class SeedData
{
    public static void AddSamplePatients(ReserveHealthContext context)
    {
        if (context.Patients.Any())
        {
            return;
        }

        var patients = new List<Patient>
        {
            new Patient
            {
                FirstName = "Jane",
                LastName = "Smith",
                DateOfBirth = new DateTime(1980, 4, 14),
                Status = "Admitted",
                MedicalHistorySummary = "Hypertension and previous cardiac review.",
                Allergies = "Penicillin"
            },

            new Patient
            {
                FirstName = "Ravi",
                LastName = "Patel",
                DateOfBirth = new DateTime(1975, 9, 21),
                Status = "Active",
                MedicalHistorySummary = "Previous cardiac investigation.",
                Allergies = "No known allergies"
            },

            new Patient
            {
                FirstName = "Sarah",
                LastName = "Wilson",
                DateOfBirth = new DateTime(1990, 2, 3),
                Status = "Follow-up",
                MedicalHistorySummary = "Previous respiratory admission.",
                Allergies = "Peanut allergy"
            },

            new Patient
            {
                FirstName = "Michael",
                LastName = "Brown",
                DateOfBirth = new DateTime(1968, 11, 8),
                Status = "Admitted",
                MedicalHistorySummary = "Type 2 diabetes and hypertension.",
                Allergies = "Sulfa medication"
            },

            new Patient
            {
                FirstName = "Aroha",
                LastName = "Williams",
                DateOfBirth = new DateTime(1987, 7, 19),
                Status = "Follow-up",
                MedicalHistorySummary = "Previous orthopaedic surgery.",
                Allergies = "No known allergies"
            },

            new Patient
            {
                FirstName = "Daniel",
                LastName = "Lee",
                DateOfBirth = new DateTime(1995, 1, 26),
                Status = "Active",
                MedicalHistorySummary = "Asthma.",
                Allergies = "Ibuprofen"
            }
        };

        context.Patients.AddRange(patients);

        context.SaveChanges();
    }
}