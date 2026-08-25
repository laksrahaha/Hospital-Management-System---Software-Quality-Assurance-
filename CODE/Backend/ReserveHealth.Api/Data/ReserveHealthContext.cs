using Microsoft.EntityFrameworkCore;
using ReserveHealth.Api.Models;

namespace ReserveHealth.Api.Data;

// This is the database context for Reserve Health.
// It connects the different hospital system models to the database.
public class ReserveHealthContext : DbContext
{
    public ReserveHealthContext(DbContextOptions<ReserveHealthContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    public DbSet<Patient> Patients { get; set; }

    public DbSet<Discharge> Discharges { get; set; }

    public DbSet<MedicationCheck> MedicationChecks { get; set; }

    public DbSet<Referral> Referrals { get; set; }

    public DbSet<DischargeTask> DischargeTasks { get; set; }
}