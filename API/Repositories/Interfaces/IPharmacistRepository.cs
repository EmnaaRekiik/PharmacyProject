using pharmacy;

namespace API.Repositories.Interfaces
{
    public interface IPharmacistRepository
    {
        Task<List<Pharmacist>> GetPharmacists();
        Task<Pharmacist> GetPharmacistById(int id);
        Task<Pharmacist> GetPharmacistByUserId(string userId);
        Task<Pharmacist> AddPharmacist(Pharmacist pharmacist);
        Task<bool> UpdatePharmacist(Pharmacist pharmacist);
        Task<bool> DeletePharmacist(int id);



    }
}
