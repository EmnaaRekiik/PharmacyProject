using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pharmacy
{
    public class PrescriptionMedication
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PrescriptionId { get; set; }
        [ForeignKey("PrescriptionId")]
        public virtual Prescription Prescription { get; set; }

        [Required]
        public int MedicationId { get; set; }
        [ForeignKey("MedicationId")]
        public virtual Medication Medication { get; set; }

        [Required]
        public int Quantity { get; set; }


        [StringLength(100)]
        public string DosageInstructions { get; set; } // e.g., "10mg twice daily"
    }
}
