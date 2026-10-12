using API.Repositories.Implementations;
using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pharmacy;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionController : ControllerBase
    {
        private readonly IPrescriptionRepository prescriptionRepository;
        private readonly IPrescriptionMedicationRepository prescriptionMedicationRepository;
        private readonly IMedicationRepository medicationRepository;
        private readonly IDispensedMedicationRepository dispensedMedicationRepository;
        private readonly IPatientRepository patientRepository;

        public PrescriptionController(
            IPrescriptionRepository prescriptionRepository,
            IPrescriptionMedicationRepository prescriptionMedicationRepository,
            IMedicationRepository medicationRepository,
            IDispensedMedicationRepository dispensedMedicationRepository,
            IPatientRepository patientRepository)
        {
            this.prescriptionRepository = prescriptionRepository;
            this.prescriptionMedicationRepository = prescriptionMedicationRepository;
            this.medicationRepository = medicationRepository;
            this.dispensedMedicationRepository = dispensedMedicationRepository;
            this.patientRepository = patientRepository;
        }

        // 🔒 SECURITY FIX: Admin sees all, Pharmacist sees only their prescriptions
        [Authorize(Roles = "Pharmacist,Admin")]
        [HttpGet]
        public async Task<IActionResult> GetPrescriptions()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (role == "Admin")
            {
                var allPrescriptions = await prescriptionRepository.GetPrescriptions();
                return Ok(allPrescriptions);
            }
            else if (role == "Pharmacist")
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var pharmacistId = await prescriptionRepository.GetPharmacistIdByUserId(userId);

                if (pharmacistId == null)
                    return NotFound("Pharmacist profile not found");

                var myPrescriptions = await prescriptionRepository.GetPrescriptionsByPharmacistId(pharmacistId.Value);
                return Ok(myPrescriptions);
            }

            return Forbid();
        }

        // 🔒 SECURITY FIX: Check prescription ownership
        [Authorize(Roles = "Pharmacist,Admin")]
        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetPrescriptionWithMedications(int id)
        {
            var prescription = await prescriptionRepository.GetPrescriptionWithPatient(id);
            if (prescription == null)
                return NotFound();

            // Check ownership if Pharmacist
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (role == "Pharmacist")
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var pharmacistId = await prescriptionRepository.GetPharmacistIdByUserId(userId);

                if (prescription.PharmacistId != pharmacistId)
                    return Forbid();
            }

            var medications = await prescriptionMedicationRepository.GetMedicationsByPrescriptionId(id);

            var details = new
            {
                prescription = prescription,
                medications = medications
            };

            return Ok(details);
        }

        // 🔒 SECURITY FIX: Check patient ownership before showing their prescriptions
        [Authorize(Roles = "Pharmacist,Admin")]
        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetPrescriptionsByPatient(int patientId)
        {
            var patient = await patientRepository.GetPatientById(patientId);
            if (patient == null)
                return NotFound("Patient not found");

            // Check patient ownership if Pharmacist
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (role == "Pharmacist")
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var pharmacistId = await prescriptionRepository.GetPharmacistIdByUserId(userId);

                if (patient.PharmacistId != pharmacistId)
                    return Forbid(); // Can't see another pharmacist's patient
            }

            var prescriptions = await prescriptionRepository.GetPrescriptionsByPatientId(patientId);
            return Ok(prescriptions);
        }

        // 🔒 ADMIN ONLY: Get prescriptions by any pharmacist
        [Authorize(Roles = "Admin")]
        [HttpGet("pharmacist/{pharmacistId:int}")]
        public async Task<IActionResult> GetPrescriptionsByPharmacist(int pharmacistId)
        {
            var prescriptions = await prescriptionRepository.GetPrescriptionsByPharmacistId(pharmacistId);
            return Ok(prescriptions);
        }

        // ✅ ALREADY SECURE: Gets current pharmacist's prescriptions
        [Authorize(Roles = "Pharmacist")]
        [HttpGet("my-prescriptions")]
        public async Task<IActionResult> GetMyPrescriptions()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var pharmacistId = await prescriptionRepository.GetPharmacistIdByUserId(userId);
            if (pharmacistId == null)
                return NotFound("Pharmacist profile not found");

            var prescriptions = await prescriptionRepository.GetPrescriptionsByPharmacistId(pharmacistId.Value);
            return Ok(prescriptions);
        }

        // 🔒 SECURITY FIX: Check patient ownership before creating prescription
        [Authorize(Roles = "Pharmacist")]
        [HttpPost]
        public async Task<IActionResult> AddPrescription(Prescription prescription)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var pharmacistId = await prescriptionRepository.GetPharmacistIdByUserId(userId);
            if (pharmacistId == null)
                return BadRequest("Pharmacist profile not found");

            // 🔒 CHECK: Can only create prescription for YOUR patients
            var patient = await patientRepository.GetPatientById(prescription.PatientId);
            if (patient == null)
                return NotFound("Patient not found");

            if (patient.PharmacistId != pharmacistId.Value)
                return Forbid(); // Can't create prescription for another pharmacist's patient

            prescription.PharmacistId = pharmacistId.Value;
            prescription.PrescriptionDate = DateTime.Now;
            prescription.Status = "Pending";

            var addedPrescription = await prescriptionRepository.AddPrescription(prescription);
            return CreatedAtAction(nameof(GetPrescriptionById), new { id = addedPrescription.PrescriptionId }, addedPrescription);
        }

        // 🔒 SECURITY FIX: Check prescription ownership
        [Authorize(Roles = "Pharmacist,Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPrescriptionById(int id)
        {
            var prescription = await prescriptionRepository.GetPrescriptionWithDetails(id);
            if (prescription == null)
                return NotFound();

            // Check ownership if Pharmacist
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (role == "Pharmacist")
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var pharmacistId = await prescriptionRepository.GetPharmacistIdByUserId(userId);

                if (prescription.PharmacistId != pharmacistId)
                    return Forbid();
            }

            return Ok(prescription);
        }

        // 🔒 SECURITY FIX: Check prescription ownership before updating
        [Authorize(Roles = "Pharmacist,Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePrescription(int id, Prescription prescription)
        {
            try
            {
                if (id != prescription.PrescriptionId)
                    return BadRequest(new { message = "ID mismatch" });

                var existing = await prescriptionRepository.GetPrescriptionWithDetails(id);
                if (existing == null)
                    return NotFound(new { message = "Prescription not found" });

                // 🔒 CHECK: Can only update YOUR prescriptions
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                if (role == "Pharmacist")
                {
                    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var pharmacistId = await prescriptionRepository.GetPharmacistIdByUserId(userId);

                    if (existing.PharmacistId != pharmacistId)
                        return Forbid();
                }

                // ✅ PREVENT changing status if already dispensed
                if (existing.Status == "Dispensed")
                {
                    return BadRequest(new { message = "Cannot modify a prescription that has already been dispensed" });
                }

                var oldStatus = existing.Status;
                var newStatus = prescription.Status;

                // Handle stock changes when changing to Dispensed
                if (oldStatus != "Dispensed" && newStatus == "Dispensed")
                {
                    var prescriptionMedications = await prescriptionMedicationRepository.GetMedicationsByPrescriptionId(id);

                    if (!prescriptionMedications.Any())
                    {
                        return BadRequest(new { message = "Cannot dispense a prescription without medications" });
                    }

                    // First check if all medications have sufficient stock
                    foreach (var pm in prescriptionMedications)
                    {
                        var medication = await medicationRepository.GetMedicationById(pm.MedicationId);
                        if (medication == null)
                        {
                            return BadRequest(new { message = $"Medication not found" });
                        }

                        if (medication.StockQuantity < pm.Quantity)
                        {
                            return BadRequest(new
                            {
                                message = $"Insufficient stock for {medication.Name}. Available: {medication.StockQuantity}, Required: {pm.Quantity}"
                            });
                        }
                    }

                    // If all checks pass, decrease stock
                    foreach (var pm in prescriptionMedications)
                    {
                        var medication = await medicationRepository.GetMedicationById(pm.MedicationId);
                        medication.StockQuantity -= pm.Quantity;
                        await medicationRepository.UpdateMedication(medication);
                    }
                }

                // Update prescription fields
                existing.PatientId = prescription.PatientId;
                existing.PrescriptionDate = prescription.PrescriptionDate;
                existing.Status = prescription.Status;
                existing.Instructions = prescription.Instructions;

                var updated = await prescriptionRepository.UpdatePrescription(existing);
                if (!updated)
                    return BadRequest(new { message = "Update failed" });

                return Ok(new { message = "Prescription updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, innerException = ex.InnerException?.Message });
            }
        }

        // ✅ DELETE ALREADY HAS SECURITY - Good job!
        [Authorize(Roles = "Pharmacist,Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrescription(int id)
        {
            try
            {
                var prescription = await prescriptionRepository.GetPrescriptionById(id);
                if (prescription == null)
                    return NotFound(new { message = "Prescription not found" });

                if (prescription.Status == "Dispensed")
                {
                    return BadRequest(new { message = "Cannot delete a dispensed prescription" });
                }

                if (User.IsInRole("Pharmacist"))
                {
                    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var pharmacistId = await prescriptionRepository.GetPharmacistIdByUserId(userId);

                    if (pharmacistId == null || prescription.PharmacistId != pharmacistId.Value)
                    {
                        return Forbid();
                    }
                }

                var result = await prescriptionRepository.DeletePrescription(id);
                if (result)
                    return Ok(new { message = "Prescription deleted successfully" });

                return BadRequest(new { message = "Failed to delete prescription" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, innerException = ex.InnerException?.Message });
            }
        }
    }
}