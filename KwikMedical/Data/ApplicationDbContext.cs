using KwikMedical.Models;
using Microsoft.EntityFrameworkCore;

namespace KwikMedical.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Patient> Patients { get; set; }
    public DbSet<EmergencyCall> EmergencyCalls { get; set; }
    public DbSet<MedicalRecord> MedicalRecords { get; set; }
    public DbSet<Hospital> Hospitals { get; set; }
    public DbSet<Ambulance> Ambulances { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Ambulance>()
            .HasOne(a => a.CurrentEmergencyCall)
            .WithOne(e => e.Ambulance)
            .HasForeignKey<Ambulance>(a => a.CurrentEmergencyCallId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Patient>()
            .HasOne(p => p.MedicalRecord)
            .WithOne(m => m.Patient)
            .HasForeignKey<MedicalRecord>(m => m.PatientId);

        modelBuilder.Entity<EmergencyCall>()
            .HasOne(e => e.Ambulance)
            .WithOne(a => a.CurrentEmergencyCall)
            .HasForeignKey<EmergencyCall>(e => e.AmbulanceId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);  // Setting Ambulance to optional

    }
}
