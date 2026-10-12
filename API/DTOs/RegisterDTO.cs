using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDTO
    {
        public string username { get; set; }
        public string password { get; set; }
        [EmailAddress]
        public string email { get; set; }
        public DateTime birthDate { get; set; }
        public string cin { get; set; }  
        public string address { get; set; }
    }
}
