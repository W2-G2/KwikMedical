using KwikMedical.Models;
using Microsoft.EntityFrameworkCore;

namespace KwikMedical.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<Ambulance> Ambulances { get; set; }
        public DbSet<EmergencyCall> EmergencyCalls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define relationships
            modelBuilder.Entity<Patient>()
                .HasOne(p => p.MedicalRecord)
                .WithOne(mr => mr.Patient)
                .HasForeignKey<MedicalRecord>(mr => mr.PatientId);

            modelBuilder.Entity<Hospital>()
                .HasMany(h => h.Ambulances)
                .WithOne(a => a.Hospital)
                .HasForeignKey(a => a.HospitalId);

            modelBuilder.Entity<MedicalRecord>()
                .HasOne(mr => mr.Ambulance)
                .WithOne(a => a.MedicalRecord)
                .HasForeignKey<Ambulance>(a => a.MedicalRecordId);

            modelBuilder.Entity<Patient>()
                .HasMany(p => p.EmergencyCalls)
                .WithOne(ec => ec.Patient)
                .HasForeignKey(ec => ec.PatientId);

            modelBuilder.Entity<Ambulance>()
                .HasOne(a => a.CurrentEmergencyCall)
                .WithOne(ec => ec.Ambulance)
                .HasForeignKey<EmergencyCall>(ec => ec.AmbulanceId);
        }
    }
}