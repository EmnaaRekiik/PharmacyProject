using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pharmacy
{
    public class Pharmacist
    {
        [Key]
        public int PharmacistId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string PharmacistName { get; set; }

        [Required]
        [StringLength(100)]
        public string PharmacyName { get; set; }

        [Required]
        [StringLength(50)]
        public string LicenseNumber { get; set; }

        [StringLength(20)]
        public string PhoneNumber { get; set; }
      
    }
}

