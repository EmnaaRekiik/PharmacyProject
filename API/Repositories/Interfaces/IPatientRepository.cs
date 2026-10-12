using pharmacy;

namespace API.Repositories.Interfaces
{
    public interface IPatientRepository
    {
        Task<List<Patient>> GetPatients();
        Task<Patient> GetPatientById(int id);
 
        Task<Patient> AddPatient(Patient patient);
        Task<bool> UpdatePatient(Patient patient);
        Task<bool> DeletePatient(int id);
        Task<List<Patient>> GetPatientsByPharmacistId(int pharmacistId);

    }

}
