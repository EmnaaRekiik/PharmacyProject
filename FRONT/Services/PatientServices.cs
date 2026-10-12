using Blazored.LocalStorage;
using pharmacy;
using System.Net.Http.Json;
namespace FRONT.Services
{
    public class PatientServices
    {
        private readonly HttpClient httpClient;
        private readonly ILocalStorageService localStorage;

        public PatientServices(HttpClient httpClient, ILocalStorageService localStorage)
        {
            this.httpClient = httpClient;
            this.localStorage = localStorage;
        }

        public async Task<List<Patient>> GetPatientsAsync()
        {
            var token = await localStorage.GetItemAsync<string>("token");
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            return await httpClient.GetFromJsonAsync<List<Patient>>("api/Patient");
        }

        public async Task<Patient> GetPatientAsync(int id)
        {
            var token = await localStorage.GetItemAsync<string>("token");
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            return await httpClient.GetFromJsonAsync<Patient>($"api/Patient/{id}");
        }

        public async Task<Patient> CreatePatient(Patient patient)
        {
            var token = await localStorage.GetItemAsync<string>("token");
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var result = await httpClient.PostAsJsonAsync<Patient>("api/Patient", patient);
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadFromJsonAsync<Patient>();
        }

        public async Task<bool> DeletePatient(int id)
        {
            var token = await localStorage.GetItemAsync<string>("token");
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var result = await httpClient.DeleteAsync($"api/Patient/{id}");
            if (result.IsSuccessStatusCode)
                return true;
            return false;
        }

        public async Task<bool> UpdatePatient(Patient patient)
        {
            var token = await localStorage.GetItemAsync<string>("token");
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var result = await httpClient.PutAsJsonAsync<Patient>("api/Patient", patient);
            if (result.IsSuccessStatusCode)
                return true;
            return false;
        }
    }
}