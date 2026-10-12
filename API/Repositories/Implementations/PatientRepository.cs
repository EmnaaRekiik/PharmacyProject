using API.Data;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using pharmacy;

namespace API.Repositories.Implementations
{
    public class PatientRepository : IPatientRepository
    {
        public readonly ApplicationContext context;

        public PatientRepository(ApplicationContext context)
        {
            this.context = context;
        }

        public async Task<Patient> AddPatient(Patient patient)
        {
            await context.Patients.AddAsync(patient);
            await context.SaveChangesAsync();
            return patient;
        }

        public async Task<bool> DeletePatient(int id)
        {
            var patient = await context.Patients.FindAsync(id);
            if (patient == null) return false;
            context.Patients.Remove(patient);
            await context.SaveChangesAsync();
            return true;
        }

        public Task GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Patient> GetPatientById(int id)
        {
            return await context.Patients.FindAsync(id);
        }

        public async Task<List<Patient>> GetPatients()
        {
            return await context.Patients.ToListAsync();
        }

        public async Task<bool> UpdatePatient(Patient patient)
        {
            var pat = await context.Patients.FindAsync(patient.PatientId);
            if (pat == null) return false;

            pat.FirstName = patient.FirstName;       
            pat.LastName = patient.LastName;         
            pat.DateOfBirth = patient.DateOfBirth;   
            pat.PhoneNumber = patient.PhoneNumber;   
            pat.Email = patient.Email;              
            pat.Address = patient.Address;           

            await context.SaveChangesAsync();
            return true;
        }
        public async Task<List<Patient>> GetPatientsByPharmacistId(int pharmacistId)
        {
            return await context.Patients
                .Where(p => p.PharmacistId == pharmacistId)
                .ToListAsync();
        }
    }
}