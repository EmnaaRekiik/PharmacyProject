using pharmacy;

namespace API.Repositories.Interfaces
{
    public interface IDispensedMedicationRepository
    {
        Task<List<DispensedMedication>> GetDispensedMedications();
        Task<List<DispensedMedication>> GetDispensedMedicationsByPharmacistId(int pharmacistId);
        Task<DispensedMedication> DispenseMedication(DispensedMedication dispensedMedication);
        Task<List<DispensedMedication>> GetDispensedMedicationsByPrescriptionId(int prescriptionId);
       
    }
}
