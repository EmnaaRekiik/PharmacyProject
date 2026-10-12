using API.Data;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using pharmacy;

namespace API.Repositories.Implementations
{
    public class MedicationRepository : IMedicationRepository
    {
        private readonly ApplicationContext context;

        public MedicationRepository(ApplicationContext context)
        {
            this.context = context;
        }

        public async Task<List<Medication>> GetMedications()
        {
            return await context.Medications.ToListAsync();
        }

        public async Task<Medication?> GetMedicationById(int id)
        {
            return await context.Medications.FindAsync(id);
        }

        public async Task<Medication> AddMedication(Medication medication)
        {
            await context.Medications.AddAsync(medication);
            await context.SaveChangesAsync();
            return medication;
        }

        public async Task<bool> UpdateMedication(Medication medication)
        {
            var existingMedication = await context.Medications.FindAsync(medication.MedicationId);
            if (existingMedication == null)
                return false;

            existingMedication.Name = medication.Name;
            existingMedication.Description = medication.Description;
            existingMedication.Price = medication.Price;
            existingMedication.StockQuantity = medication.StockQuantity;
    

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteMedication(int id)
        {
            var medication = await context.Medications.FindAsync(id);
            if (medication == null)
                return false;

            context.Medications.Remove(medication);
            await context.SaveChangesAsync();
            return true;
        }

        // New method for pharmacist to update only stock
        public async Task<bool> UpdateMedicationStock(int id, int stockQuantity)
        {
            var medication = await context.Medications.FindAsync(id);
            if (medication == null)
                return false;

            medication.StockQuantity = stockQuantity;
            await context.SaveChangesAsync();
            return true;
        }
    }
}