using API.Repositories.Implementations;
using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Migrations;
using pharmacy;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Pharmacist,Admin")]
    public class DispensedMedicationController : ControllerBase
    {
        private readonly IDispensedMedicationRepository dispensedMedicationRepository;
        private readonly IMedicationRepository medicationRepository;
        private readonly IPrescriptionRepository prescriptionRepository;
        private readonly IPharmacistRepository pharmacistRepository; 

        public DispensedMedicationController(
            IDispensedMedicationRepository dispensedMedicationRepository,
            IMedicationRepository medicationRepository,
            IPrescriptionRepository prescriptionRepository,
            IPharmacistRepository pharmacistRepository) // Add this parameter
        {
            this.dispensedMedicationRepository = dispensedMedicationRepository;
            this.medicationRepository = medicationRepository;
            this.prescriptionRepository = prescriptionRepository;
            this.pharmacistRepository = pharmacistRepository; // Add this
        }

        [HttpGet]
        public async Task<IActionResult> GetDispensedMedications()
        {
            var dispensedMedications = await dispensedMedicationRepository.GetDispensedMedications();
            return Ok(dispensedMedications);
        }

        [HttpGet("pharmacist/{pharmacistId}")]
        public async Task<IActionResult> GetDispensedMedicationsByPharmacist(int pharmacistId)
        {
            var dispensedMedications = await dispensedMedicationRepository.GetDispensedMedicationsByPharmacistId(pharmacistId);
            return Ok(dispensedMedications);
        }

        [HttpPost("dispense")]
        public async Task<IActionResult> DispenseMedication(DispensedMedication dispensedMedication)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var pharmacist = await pharmacistRepository.GetPharmacistByUserId(userId);
            if (pharmacist == null)
            {
                return NotFound("Pharmacist profile not found");
            }

            // Get the medication to check stock
            var medication = await medicationRepository.GetMedicationById(dispensedMedication.MedicationId);
            if (medication == null)
                return NotFound("Medication not found");

            // Check if enough stock is available
            if (medication.StockQuantity < dispensedMedication.Quantity)
            {
                return BadRequest($"Insufficient stock. Available: {medication.StockQuantity}, Requested: {dispensedMedication.Quantity}");
            }

            dispensedMedication.PharmacistId = pharmacist.PharmacistId;
            dispensedMedication.DeliveryDate = DateTime.Now;

            var dispensed = await dispensedMedicationRepository.DispenseMedication(dispensedMedication);

            // Decrease stock
            medication.StockQuantity -= dispensedMedication.Quantity;
            await medicationRepository.UpdateMedication(medication);

            // Update prescription status to "Dispensed"
            var prescription = await prescriptionRepository.GetPrescriptionById(dispensedMedication.PrescriptionId);
            if (prescription != null)
            {
                prescription.Status = "Dispensed";
                await prescriptionRepository.UpdatePrescription(prescription);
            }

            return Ok(new
            {
                message = "Medication dispensed successfully",
                dispensedId = dispensed.DispensedId,
                remainingStock = medication.StockQuantity
            });
        }
    }
}
