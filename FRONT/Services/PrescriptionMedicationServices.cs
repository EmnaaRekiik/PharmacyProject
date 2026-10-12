using Blazored.LocalStorage;
using pharmacy;
using System.Net.Http.Json;

namespace FRONT.Services
{
    public class PrescriptionMedicationServices
    {
        private readonly HttpClient httpClient;
        private readonly ILocalStorageService localStorage;

        public PrescriptionMedicationServices(HttpClient httpClient, ILocalStorageService localStorage)
        {
            this.httpClient = httpClient;
            this.localStorage = localStorage;
        }

        public async Task<PrescriptionMedication> AddPrescriptionMedication(CreatePrescriptionMedicationDto dto)
        {
            try
            {
                var token = await localStorage.GetItemAsync<string>("token");
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await httpClient.PostAsJsonAsync("api/PrescriptionMedication", dto);

                if (response.IsSuccessStatusCode)
                {
                    // If using the improved controller, it returns the full PrescriptionMedication object
                    return await response.Content.ReadFromJsonAsync<PrescriptionMedication>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to add medication. Status: {response.StatusCode}, Error: {errorContent}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception adding prescription medication: {ex.Message}");
                return null;
            }
        }

        public async Task<List<PrescriptionMedication>> GetMedicationsByPrescriptionId(int prescriptionId)
        {
            try
            {
                var token = await localStorage.GetItemAsync<string>("token");
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await httpClient.GetAsync($"api/PrescriptionMedication/prescription/{prescriptionId}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<PrescriptionMedication>>();
                }
                else
                {
                    Console.WriteLine($"Failed to get medications. Status: {response.StatusCode}");
                    return new List<PrescriptionMedication>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception getting medications: {ex.Message}");
                return new List<PrescriptionMedication>();
            }
        }

        public async Task<bool> DeletePrescriptionMedication(int id)
        {
            try
            {
                var token = await localStorage.GetItemAsync<string>("token");
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await httpClient.DeleteAsync($"api/PrescriptionMedication/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to delete medication. Status: {response.StatusCode}, Error: {errorContent}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception deleting medication: {ex.Message}");
                return false;
            }
        }
    }
}