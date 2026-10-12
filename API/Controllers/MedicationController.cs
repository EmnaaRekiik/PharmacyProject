using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pharmacy;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicationController : ControllerBase
    {
        private readonly IMedicationRepository medicationRepository;

        public MedicationController(IMedicationRepository medicationRepository)
        {
            this.medicationRepository = medicationRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Pharmacist,Admin")]
        public async Task<ActionResult> GetMedications()
        {
            var medications = await medicationRepository.GetMedications();
            return Ok(medications);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Pharmacist,Admin")]
        public async Task<ActionResult> GetMedicationById(int id)
        {
            var medication = await medicationRepository.GetMedicationById(id);
            if (medication == null)
                return NotFound();
            return Ok(medication);
        }

        [HttpGet("search")]
        [Authorize(Roles = "Pharmacist,Admin")]
        public async Task<ActionResult> SearchMedications(string name)
        {
            var medications = await medicationRepository.GetMedications();
            var filtered = medications.Where(m => m.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
            return Ok(filtered);
        }

        // ADMIN ONLY - Full CRUD operations
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AddMedication(Medication medication)
        {
            var medications = await medicationRepository.GetMedications();
            if (medications.Any(m => m.Name.Equals(medication.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest("Medication already exists");
            }

            var addedMedication = await medicationRepository.AddMedication(medication);
            return CreatedAtAction(nameof(GetMedicationById), new { id = addedMedication.MedicationId }, addedMedication);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateMedication(Medication medication)
        {
            var updated = await medicationRepository.UpdateMedication(medication);
            if (!updated)
                return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteMedication(int id)
        {
            var result = await medicationRepository.DeleteMedication(id);
            if (result)
                return NoContent();
            return NotFound("Medication not found");
        }

        // PHARMACIST ONLY - Update stock quantity
        [HttpPatch("{id}/stock")]
        [Authorize(Roles = "Pharmacist")]
        public async Task<ActionResult> UpdateMedicationStock(int id, [FromBody] UpdateStockDto stockDto)
        {
            var result = await medicationRepository.UpdateMedicationStock(id, stockDto.StockQuantity);
            if (!result)
                return NotFound("Medication not found");

            return Ok("Stock updated successfully");
        }
    }

    // DTO for stock updates
    public class UpdateStockDto
    {
        public int StockQuantity { get; set; }
    }
}