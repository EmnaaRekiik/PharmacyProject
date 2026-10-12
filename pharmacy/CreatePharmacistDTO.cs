using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pharmacy
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


