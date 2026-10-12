using Microsoft.Ajax.Utilities;
using pharmacy;

namespace API.Repositories.Interfaces
{
    public interface IPrescriptionRepository
    {
        Task<List<Prescription>> GetPrescriptions();
        Task<Prescription> GetPrescriptionById(int id);
        Task<Prescription> GetPrescriptionWithDetails(int id);
        Task<Prescription> GetPrescriptionWithPatient(int id);
        Task<List<Prescription>> GetPrescriptionsByPatientId(int patientId);
        Task<List<Prescription>> GetPrescriptionsByPharmacistId(int pharmacistId);
        Task<int?> GetPharmacistIdByUserId(string userId);
        Task<Prescription> AddPrescription(Prescription prescription);
        Task<bool> UpdatePrescription(Prescription prescription);
        Task<bool> DeletePrescription(int id);



    }
}