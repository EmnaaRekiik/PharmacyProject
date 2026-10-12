using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pharmacy
{
    public class CreatePrescriptionDto
    {
        public int PatientId { get; set; }
        public string Instructions { get; set; }
    }
}
