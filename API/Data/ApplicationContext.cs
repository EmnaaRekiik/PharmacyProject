using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using pharmacy;

namespace API.Data
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Pharmacist> Pharmacists { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<PrescriptionMedication> PrescriptionMedications { get; set; }
        public DbSet<DispensedMedication> DispensedMedications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🔧 ADD THESE THREE CONFIGURATIONS to fix the cascade delete error
            modelBuilder.Entity<Patient>()
                .HasOne(p => p.Pharmacist)
                .WithMany()
                .HasForeignKey(p => p.PharmacistId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Patient)
                .WithMany(p => p.Prescriptions)
                .HasForeignKey(p => p.PatientId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Pharmacist)
                .WithMany()
                .HasForeignKey(p => p.PharmacistId)
                .OnDelete(DeleteBehavior.NoAction);

            // Your existing configurations
            modelBuilder.Entity<DispensedMedication>()
                .HasOne(d => d.Prescription)
                .WithMany()
                .HasForeignKey(d => d.PrescriptionId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<DispensedMedication>()
                .HasOne(d => d.Medication)
                .WithMany()
                .HasForeignKey(d => d.MedicationId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<DispensedMedication>()
                .HasOne(d => d.Pharmacist)
                .WithMany()
                .HasForeignKey(d => d.PharmacistId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}