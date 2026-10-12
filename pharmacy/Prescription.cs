using pharmacy;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Prescription
{
    [Key]
    public int PrescriptionId { get; set; }

    [Required]
    public int PatientId { get; set; }
    [ForeignKey("PatientId")]
    public virtual Patient? Patient { get; set; }

    [Required]
    public int PharmacistId { get; set; }  // INT - references Pharmacist.PharmacistId
    [ForeignKey("PharmacistId")]
    public virtual Pharmacist? Pharmacist { get; set; }

    [Required]
    public DateTime PrescriptionDate { get; set; }

    [StringLength(500)]
    public string Instructions { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = "Pending";

    public virtual ICollection<PrescriptionMedication>? PrescriptionMedications { get; set; }
}