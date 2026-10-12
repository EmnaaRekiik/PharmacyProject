using API.Data;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using pharmacy;

namespace API.Repositories.Implementations
{
    public class PrescriptionRepository : IPrescriptionRepository
    {
        public readonly ApplicationContext context;

        public PrescriptionRepository(ApplicationContext context)
        {
            this.context = context;
        }

        public async Task<Prescription> AddPrescription(Prescription prescription)
        {
            await context.Prescriptions.AddAsync(prescription);
            await context.SaveChangesAsync();
            return prescription;
        }

        public async Task<bool> DeletePrescription(int id)
        {
            var prescription = await context.Prescriptions.FindAsync(id);
            if (prescription == null) return false;
            context.Prescriptions.Remove(prescription);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<Prescription> GetPrescriptionById(int id)
        {
            return await context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Pharmacist)
                .Include(p => p.PrescriptionMedications)
                    .ThenInclude(pm => pm.Medication)
                .FirstOrDefaultAsync(p => p.PrescriptionId == id);
        }

        public async Task<Prescription> GetPrescriptionWithPatient(int id)
        {
            return await context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Pharmacist)
                .Include(p => p.PrescriptionMedications)
                    .ThenInclude(pm => pm.Medication)
                .FirstOrDefaultAsync(p => p.PrescriptionId == id);
        }

        public async Task<List<Prescription>> GetPrescriptions()
        {
            return await context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Pharmacist)
                .OrderByDescending(p => p.PrescriptionDate)
                .ToListAsync();
        }

        public async Task<List<Prescription>> GetPrescriptionsByPatientId(int patientId)
        {
            return await context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Pharmacist)
                .Where(p => p.PatientId == patientId)
                .ToListAsync();
        }

        // Changed parameter from string to int
        public async Task<List<Prescription>> GetPrescriptionsByPharmacistId(int pharmacistId)
        {
            return await context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Pharmacist)
                .Where(p => p.PharmacistId == pharmacistId)
                .ToListAsync();
        }

        public async Task<bool> UpdatePrescription(Prescription prescription)
        {
            try
            {
                var existing = await context.Prescriptions.FindAsync(prescription.PrescriptionId);
                if (existing == null)
                    return false;

                // Update only the fields that should change
                existing.PatientId = prescription.PatientId;
                existing.PrescriptionDate = prescription.PrescriptionDate;
                existing.Status = prescription.Status;
                existing.Instructions = prescription.Instructions;
                // DON'T update PharmacistId - it should stay the same

                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Update error: {ex.Message}");
                return false;
            }
        }

        // New method to get PharmacistId by UserId
        public async Task<int?> GetPharmacistIdByUserId(string userId)
        {
            var pharmacist = await context.Pharmacists
                .FirstOrDefaultAsync(p => p.UserId == userId);

            return pharmacist?.PharmacistId;
        }
        public async Task<Prescription> GetPrescriptionWithDetails(int id)
        {
            return await context.Prescriptions
                .Include(p => p.Patient)
                .Include(p => p.Pharmacist)
                .Include(p => p.PrescriptionMedications)
                    .ThenInclude(pm => pm.Medication)
                .FirstOrDefaultAsync(p => p.PrescriptionId == id);
        }
        
        

    }
}