//using System.Net.Http.Json;
//using pharmacy;

//namespace FRONT.Services
//{
//    public class AdminService
//    {
//        private readonly HttpClient httpClient;

//        public AdminService(HttpClient httpClient)
//        {
//            this.httpClient = httpClient;
//        }

//        // Profile methods
//        public async Task<AdminProfileDto?> GetProfile()
//        {
//            try
//            {
//                return await httpClient.GetFromJsonAsync<AdminProfileDto>("api/Admin/profile");
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error getting profile: {ex.Message}");
//                return null;
//            }
//        }

//        public async Task<bool> UpdateProfile(UpdateProfileDto profile)
//        {
//            try
//            {
//                var response = await httpClient.PutAsJsonAsync("api/Admin/profile", profile);
//                return response.IsSuccessStatusCode;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error updating profile: {ex.Message}");
//                return false;
//            }
//        }

//        // Dispensed medications history
//        public async Task<List<DispensedMedication>> GetDispensedMedicationsHistory()
//        {
//            try
//            {
//                var result = await httpClient.GetFromJsonAsync<List<DispensedMedication>>("api/Admin/dispensed-medications");
//                return result ?? new List<DispensedMedication>();
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error getting dispensed medications: {ex.Message}");
//                return new List<DispensedMedication>();
//            }
//        }
//    }
//}
