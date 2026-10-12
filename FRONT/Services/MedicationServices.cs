using Blazored.LocalStorage;
using pharmacy;
using System.Net.Http.Json;
using System.Text.Json;

namespace FRONT.Services
{
    public class MedicationServices
    {
        private readonly HttpClient httpClient;
        private readonly ILocalStorageService localStorage;

        public MedicationServices(HttpClient httpClient, ILocalStorageService localStorage)
        {
            this.httpClient = httpClient;
            this.localStorage = localStorage;
        }

        public async Task<List<Medication>> GetMedicationsAsync()
        {
            var token = await localStorage.GetItemAsync<string>("token");
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            return await httpClient.GetFromJsonAsync<List<Medication>>("api/Medication");
        }

        public async Task<Medication> GetMedicationAsync(int id)
        {
            var token = await localStorage.GetItemAsync<string>("token");
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            return await httpClient.GetFromJsonAsync<Medication>($"api/Medication/{id}");
        }

        public async Task<Medication> CreateMedication(Medication medication)
        {
            var token = await localStorage.GetItemAsync<string>("token");
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var result = await httpClient.PostAsJsonAsync<Medication>("api/Medication", medication);
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadFromJsonAsync<Medication>();
        }

        public async Task<bool> DeleteMedication(int id)
        {
            var token = await localStorage.GetItemAsync<string>("token");
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var result = await httpClient.DeleteAsync($"api/Medication/{id}");
            return result.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateMedication(Medication medication)
        {
            var token = await localStorage.GetItemAsync<string>("token");
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var result = await httpClient.PutAsJsonAsync<Medication>("api/Medication", medication);
            return result.IsSuccessStatusCode;
        }

        // New method for pharmacist to update stock only
        public async Task<bool> UpdateMedicationStock(int id, int stockQuantity)
        {
            var token = await localStorage.GetItemAsync<string>("token");
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var dto = new { StockQuantity = stockQuantity };
            var response = await httpClient.PatchAsync($"api/Medication/{id}/stock",
                new StringContent(JsonSerializer.Serialize(dto), System.Text.Encoding.UTF8, "application/json"));

            return response.IsSuccessStatusCode;
        }
    }
}