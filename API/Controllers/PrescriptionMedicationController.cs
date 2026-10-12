using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pharmacy;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Pharmacist,Admin")]
    public class PrescriptionMedicationController : ControllerBase
    {
        private readonly IPrescriptionMedicationRepository prescriptionMedicationRepository;

        public PrescriptionMedicationController(IPrescriptionMedicationRepository prescriptionMedicationRepository)
        {
            this.prescriptionMedicationRepository = prescriptionMedicationRepository;
        }

        [HttpGet("prescription/{prescriptionId}")]
        public async Task<IActionResult> GetMedicationsByPrescription(int prescriptionId)
        {
            try
            {
                var medications = await prescriptionMedicationRepository.GetMedicationsByPrescriptionId(prescriptionId);
                return Ok(medications);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddPrescriptionMedication(CreatePrescriptionMedicationDto dto)
        {
            try
            {
                // Validate input
                if (dto.PrescriptionId <= 0)
                    return BadRequest(new { error = "Invalid PrescriptionId" });

                if (dto.MedicationId <= 0)
                    return BadRequest(new { error = "Invalid MedicationId" });

                if (dto.Quantity <= 0)
                    return BadRequest(new { error = "Quantity must be greater than 0" });

                if (string.IsNullOrWhiteSpace(dto.DosageInstructions))
                    return BadRequest(new { error = "Dosage instructions are required" });

                var prescriptionMedication = new PrescriptionMedication
                {
                    PrescriptionId = dto.PrescriptionId,
                    MedicationId = dto.MedicationId,
                    Quantity = dto.Quantity,
                    DosageInstructions = dto.DosageInstructions
                };

                var added = await prescriptionMedicationRepository.AddPrescriptionMedication(prescriptionMedication);

                if (added == null)
                    return BadRequest(new { error = "Failed to add medication to prescription" });

                // Return the full object instead of just message and id
                return Ok(added);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message, details = ex.InnerException?.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrescriptionMedication(int id)
        {
            try
            {
                var result = await prescriptionMedicationRepository.DeletePrescriptionMedication(id);

                if (result)
                    return NoContent();

                return NotFound(new { error = "Prescription medication not found or already deleted" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}