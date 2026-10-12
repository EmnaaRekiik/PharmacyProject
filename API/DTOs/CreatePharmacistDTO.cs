using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class CreatePharmacistDTO
    {
        public string username { get; set; }
        [EmailAddress]
        public string email { get; set; }
        public string password { get; set; }
        public string pharmacistName { get; set; }
        public string pharmacyName { get; set; }
        public string licenseNumber { get; set; }
        public string phoneNumber { get; set; }
    }
}
