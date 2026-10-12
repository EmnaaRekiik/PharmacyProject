using Blazored.LocalStorage;
using pharmacy;
using System.Net.Http.Json;

namespace FRONT.Services
{
    public class DispensedMedicationServices
    {
        private readonly HttpClient httpClient;
        private readonly ILocalStorageService localStorage;

        public DispensedMedicationServices(HttpClient httpClient, ILocalStorageService localStorage)
        {
            this.httpClient = httpClient;
            this.localStorage = localStorage;
        }

        public async Task<List<DispensedMedication>> GetDispensedMedicationsAsync()
        {
            var token = await localStorage.GetItemAsync<string>("token");
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            return await httpClient.GetFromJsonAsync<List<DispensedMedication>>("api/DispensedMedication");
        }

        public async Task<List<DispensedMedication>> GetDispensedMedicationsByPharmacistAsync(int pharmacistId)
        {
            var token = await localStorage.GetItemAsync<string>("token");
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            return await httpClient.GetFromJsonAsync<List<DispensedMedication>>($"api/DispensedMedication/pharmacist/{pharmacistId}");
        }
    }
}