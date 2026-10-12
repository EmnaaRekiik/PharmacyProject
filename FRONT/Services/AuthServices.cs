
using Blazored.LocalStorage;
using pharmacy;
using System.Net.Http.Json;
namespace FRONT.Services
{
    
        public class AuthServices
        {
            private readonly HttpClient httpClient;
            private readonly ILocalStorageService localStorage;
            public AuthServices(HttpClient httpClient, ILocalStorageService localStorage)
            {
                this.httpClient = httpClient;
                this.localStorage = localStorage;
            }
            public async Task<bool> login(LoginDTO loginDTO)
            {
                var response = await httpClient.PostAsJsonAsync("api/Account/Login", loginDTO);
                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    if (loginResponse != null)
                    {
                        await localStorage.SetItemAsync("token", loginResponse.token);
                        await localStorage.SetItemAsync("role", loginResponse.role);
                        await localStorage.SetItemAsync("userId", loginResponse.userId);
                        await localStorage.SetItemAsync("username", loginResponse.username);
                        return true;
                    }
                }
                return false;
            }
            public async Task logout()
            {
                await localStorage.RemoveItemAsync("token");
                await localStorage.RemoveItemAsync("role");
                await localStorage.RemoveItemAsync("userId");
                await localStorage.RemoveItemAsync("username");
            }
            public async Task<string> GetRole()
            {
                return await localStorage.GetItemAsync<string>("role");
            }
            public async Task<string> GetUserId()
            {
                return await localStorage.GetItemAsync<string>("userId");
            }
            public async Task<string> GetUsername()
            {
                return await localStorage.GetItemAsync<string>("username");
            }
            public async Task<bool> IsAuthenticated()
            {
                var token = await localStorage.GetItemAsync<string>("token");
                return !string.IsNullOrEmpty(token);
            }
        }
        public class LoginResponse
        {
            public string token { get; set; }
            public DateTime expiration { get; set; }
            public string username { get; set; }
            public string role { get; set; }
            public string userId { get; set; }
        }
    }
