using API.Data;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using pharmacy;

namespace API.Repositories.Implementations
{
    public class DispensedMedicationRepository : IDispensedMedicationRepository
    {
        public readonly ApplicationContext context;

        public DispensedMedicationRepository(ApplicationContext context)
        {
            this.context = context;
        }

        public async Task<DispensedMedication> DispenseMedication(DispensedMedication dispensedMedication)
        {
            await context.DispensedMedications.AddAsync(dispensedMedication);
            await context.SaveChangesAsync();
            return dispensedMedication;
        }

        public async Task<List<DispensedMedication>> GetDispensedMedications()
        {
            return await context.DispensedMedications
                .Include(d => d.Prescription)
                    .ThenInclude(p => p.Patient)
                .Include(d => d.Medication)
                .Include(d => d.Pharmacist)
                .OrderByDescending(d => d.DeliveryDate)
                .ToListAsync();
        }

        // Changed parameter to INT
        public async Task<List<DispensedMedication>> GetDispensedMedicationsByPharmacistId(int pharmacistId)
        {
            return await context.DispensedMedications
                .Include(d => d.Prescription)
                    .ThenInclude(p => p.Patient)
                .Include(d => d.Medication)
                .Include(d => d.Pharmacist)
                .Where(d => d.PharmacistId == pharmacistId)
                .OrderByDescending(d => d.DeliveryDate)
                .ToListAsync();
        }
        public async Task<List<DispensedMedication>> GetDispensedMedicationsByPrescriptionId(int prescriptionId)
        {
            return await context.DispensedMedications
                .Where(dm => dm.PrescriptionId == prescriptionId)
                .Include(dm => dm.Medication)
                .ToListAsync();
        }

    }

}