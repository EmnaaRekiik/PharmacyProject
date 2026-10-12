using pharmacy;
using System.Net.Http.Json;

namespace FRONT.Services
{
    public class PrescriptionServices
    {
        private readonly HttpClient httpClient;

        public PrescriptionServices(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<List<Prescription>> GetPrescriptionsAsync()
        {
            try
            {
                return await httpClient.GetFromJsonAsync<List<Prescription>>("api/Prescription");
            }
            catch
            {
                return new List<Prescription>();
            }
        }

        public async Task<Prescription> GetPrescriptionAsync(int id)
        {
            try
            {
                var response = await httpClient.GetAsync($"api/Prescription/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Prescription>();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<Prescription>> GetPrescriptionsByPatientAsync(int patientId)
        {
            try
            {
                var response = await httpClient.GetAsync($"api/Prescription/patient/{patientId}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<Prescription>>();
                }

                return new List<Prescription>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching prescriptions by patient: {ex.Message}");
                return new List<Prescription>();
            }
        }

        public async Task<Prescription> CreatePrescription(Prescription prescription)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync("api/Prescription", prescription);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Prescription>();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdatePrescription(Prescription prescription)
        {
            try
            {
                var updateDto = new
                {
                    prescriptionId = prescription.PrescriptionId,
                    patientId = prescription.PatientId,
                    pharmacistId = prescription.PharmacistId,
                    prescriptionDate = prescription.PrescriptionDate,
                    status = prescription.Status,
                    instructions = prescription.Instructions
                };

                var response = await httpClient.PutAsJsonAsync($"api/Prescription/{prescription.PrescriptionId}", updateDto);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Update failed: {error}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during update: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeletePrescription(int id)
        {
            try
            {
                var response = await httpClient.DeleteAsync($"api/Prescription/{id}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}