using API.Repositories.Interfaces;
using API.Data;
using Microsoft.EntityFrameworkCore;
using pharmacy;

namespace API.Repositories.Implementations
{
    public class PrescriptionMedicationRepository : IPrescriptionMedicationRepository
    {
        public readonly ApplicationContext context;

        public PrescriptionMedicationRepository(ApplicationContext context)
        {
            this.context = context;
        }

        public async Task<PrescriptionMedication> AddPrescriptionMedication(PrescriptionMedication prescriptionMedication)
        {
            await context.PrescriptionMedications.AddAsync(prescriptionMedication);
            await context.SaveChangesAsync();
            return prescriptionMedication;
        }

        public async Task<bool> DeletePrescriptionMedication(int id)
        {
            var prescriptionMedication = await context.PrescriptionMedications.FindAsync(id);
            if (prescriptionMedication == null) return false;
            context.PrescriptionMedications.Remove(prescriptionMedication);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<List<PrescriptionMedication>> GetMedicationsByPrescriptionId(int prescriptionId)
        {
            return await context.PrescriptionMedications
                .Where(pm => pm.PrescriptionId == prescriptionId)
                .Include(pm => pm.Medication)
                .ToListAsync();
        }
    }
}
