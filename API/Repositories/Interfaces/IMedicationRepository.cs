using pharmacy;

namespace API.Repositories.Interfaces
{
    public interface IMedicationRepository
    {
        Task<List<Medication>> GetMedications();
        Task<Medication?> GetMedicationById(int id);
        Task<Medication> AddMedication(Medication medication);
        Task<bool> UpdateMedication(Medication medication);
        Task<bool> DeleteMedication(int id);
        Task<bool> UpdateMedicationStock(int id, int stockQuantity); // New method for pharmacist
    }
}