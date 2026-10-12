using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pharmacy
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(200)]
        public string Address { get; set; }

        // ADD THESE LINES - Link patient to pharmacist
        [Required]
        public int PharmacistId { get; set; }

        [ForeignKey("PharmacistId")]
        public virtual Pharmacist? Pharmacist { get; set; }

        // Navigation property for prescriptions
        public virtual ICollection<Prescription>? Prescriptions { get; set; }
    }
}