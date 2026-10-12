using pharmacy;

namespace API.Repositories.Interfaces
{
    public interface IPrescriptionMedicationRepository
    {
        Task<List<PrescriptionMedication>> GetMedicationsByPrescriptionId(int prescriptionId);
        Task<PrescriptionMedication> AddPrescriptionMedication(PrescriptionMedication prescriptionMedication);
        Task<bool> DeletePrescriptionMedication(int id);
    }
}
