using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pharmacy
{
    public class DispensedMedication
    {
        [Key]
        public int DispensedId { get; set; }

        [Required]
        public int PrescriptionId { get; set; }
        [ForeignKey("PrescriptionId")]
        public virtual Prescription? Prescription { get; set; }

        [Required]
        public int MedicationId { get; set; }
        [ForeignKey("MedicationId")]
        public virtual Medication? Medication { get; set; }

        [Required]
        public int PharmacistId { get; set; }  // INT - references Pharmacist.PharmacistId
        [ForeignKey("PharmacistId")]
        public virtual Pharmacist? Pharmacist { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public DateTime DeliveryDate { get; set; }
    }
}