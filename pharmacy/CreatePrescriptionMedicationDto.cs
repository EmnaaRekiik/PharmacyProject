using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pharmacy
{
    public class CreatePrescriptionMedicationDto
    {
        public int PrescriptionId { get; set; }
        public int MedicationId { get; set; }
        public int Quantity { get; set; }
        public string DosageInstructions { get; set; }
    }
}
