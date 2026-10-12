using API.Repositories.Interfaces;
using API.Data;

using Microsoft.EntityFrameworkCore;
using pharmacy;

namespace API.Repositories.Implementations
{
    public class PharmacistRepository : IPharmacistRepository
    {
        public readonly ApplicationContext context;

        public PharmacistRepository(ApplicationContext context)
        {
            this.context = context;
        }

        public async Task<Pharmacist> AddPharmacist(Pharmacist pharmacist)
        {
            await context.Pharmacists.AddAsync(pharmacist);
            await context.SaveChangesAsync();
            return pharmacist;
        }

        public async Task<bool> DeletePharmacist(int id)
        {
            var pharmacist = await context.Pharmacists.FindAsync(id);
            if (pharmacist == null) return false;
            context.Pharmacists.Remove(pharmacist);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<Pharmacist> GetPharmacistById(int id)
        {
            return await context.Pharmacists.FindAsync(id);
        }

        public async Task<Pharmacist> GetPharmacistByUserId(string userId)
        {
            return await context.Pharmacists.FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<List<Pharmacist>> GetPharmacists()
        {
            return await context.Pharmacists.ToListAsync();
        }

        public async Task<bool> UpdatePharmacist(Pharmacist pharmacist)
        {
            var phar = await context.Pharmacists.FindAsync(pharmacist.PharmacistId);
            if (phar == null) return false;
            phar.PharmacistName = pharmacist.PharmacistName;
            phar.PharmacyName = pharmacist.PharmacyName;
            phar.LicenseNumber = pharmacist.LicenseNumber;
            phar.PhoneNumber = pharmacist.PhoneNumber;
            await context.SaveChangesAsync();
            return true;
        }

    }
}
