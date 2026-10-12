using Blazored.LocalStorage;
using pharmacy;
using System.Net.Http.Json;

namespace FRONT.Services
{
    public class PharmacistServices
    {
        private readonly HttpClient httpClient;
        private readonly ILocalStorageService localStorage;

        public PharmacistServices(HttpClient httpClient, ILocalStorageService localStorage)
        {
            this.httpClient = httpClient;
            this.localStorage = localStorage;
        }

        public async Task<List<Pharmacist>> GetPharmacistsAsync()
        {
            var token = await localStorage.GetItemAsync<string>("token");
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            return await httpClient.GetFromJsonAsync<List<Pharmacist>>("api/Pharmacist/pharmacists");
        }

        public async Task<Pharmacist> GetPharmacistAsync(int id)
        {
            var token = await localStorage.GetItemAsync<string>("token");
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            return await httpClient.GetFromJsonAsync<Pharmacist>($"api/Pharmacist/{id}");
        }

        public async Task<Pharmacist> CreatePharmacist(CreatePharmacistDTO pharmacist)
        {
            var token = await localStorage.GetItemAsync<string>("token");
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var result = await httpClient.PostAsJsonAsync("api/Pharmacist/create-pharmacist", pharmacist);

            if (!result.IsSuccessStatusCode)
            {
                // Get the actual error message from the API
                var errorContent = await result.Content.ReadAsStringAsync();
                throw new Exception($"API Error: {errorContent}");
            }

            return await result.Content.ReadFromJsonAsync<Pharmacist>();
        }

        public async Task<bool> DeletePharmacist(int id)
        {
            var token = await localStorage.GetItemAsync<string>("token");
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var result = await httpClient.DeleteAsync($"api/Pharmacist/pharmacist/{id}");
            if (result.IsSuccessStatusCode)
                return true;
            return false;
        }

        public async Task<bool> UpdatePharmacist(Pharmacist pharmacist)
        {
            var token = await localStorage.GetItemAsync<string>("token");
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var result = await httpClient.PutAsJsonAsync<Pharmacist>("api/Pharmacist/update", pharmacist);
            if (result.IsSuccessStatusCode)
                return true;
            return false;
        }
    }
}
