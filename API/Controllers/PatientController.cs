using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pharmacy;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientRepository patientRepository;
        private readonly IPrescriptionRepository prescriptionRepository;
        private readonly IPrescriptionMedicationRepository prescriptionMedicationRepository;
        private readonly IPharmacistRepository pharmacistRepository;

        public PatientController(
            IPatientRepository patientRepository,
            IPrescriptionRepository prescriptionRepository,
            IPrescriptionMedicationRepository prescriptionMedicationRepository,
            IPharmacistRepository pharmacistRepository)
        {
            this.patientRepository = patientRepository;
            this.prescriptionRepository = prescriptionRepository;
            this.prescriptionMedicationRepository = prescriptionMedicationRepository;
            this.pharmacistRepository = pharmacistRepository;
        }

        // Admin sees ALL patients, Pharmacist sees only THEIR patients
        [Authorize(Roles = "Pharmacist,Admin")]
        [HttpGet]
        public async Task<IActionResult> GetPatients()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (role == "Admin")
            {
                // Admin can see all patients
                var allPatients = await patientRepository.GetPatients();
                return Ok(allPatients);
            }
            else if (role == "Pharmacist")
            {
                // Pharmacist only sees their own patients
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var pharmacist = await pharmacistRepository.GetPharmacistByUserId(userId);

                if (pharmacist == null)
                    return NotFound("Pharmacist not found");

                var myPatients = await patientRepository.GetPatientsByPharmacistId(pharmacist.PharmacistId);
                return Ok(myPatients);
            }

            return Forbid();
        }

        [Authorize(Roles = "Pharmacist,Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatientById(int id)
        {
            var patient = await patientRepository.GetPatientById(id);
            if (patient == null)
                return NotFound();

            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            // If pharmacist, check they own this patient
            if (role == "Pharmacist")
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var pharmacist = await pharmacistRepository.GetPharmacistByUserId(userId);

                if (patient.PharmacistId != pharmacist.PharmacistId)
                    return Forbid(); // 403 - Not your patient!
            }

            return Ok(patient);
        }

        [Authorize(Roles = "Pharmacist,Admin")]
        [HttpGet("{id}/medications")]
        public async Task<IActionResult> GetPatientMedications(int id)
        {
            var patient = await patientRepository.GetPatientById(id);
            if (patient == null)
                return NotFound();

            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            // If pharmacist, check they own this patient
            if (role == "Pharmacist")
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var pharmacist = await pharmacistRepository.GetPharmacistByUserId(userId);

                if (patient.PharmacistId != pharmacist.PharmacistId)
                    return Forbid();
            }

            var prescriptions = await prescriptionRepository.GetPrescriptionsByPatientId(id);
            var allMedications = new List<object>();

            foreach (var prescription in prescriptions)
            {
                var meds = await prescriptionMedicationRepository.GetMedicationsByPrescriptionId(prescription.PrescriptionId);
                allMedications.AddRange(meds.Select(m => new
                {
                    prescriptionId = prescription.PrescriptionId,
                    prescriptionDate = prescription.PrescriptionDate,
                    status = prescription.Status,
                    medication = m
                }));
            }

            return Ok(new
            {
                patient = patient,
                medications = allMedications
            });
        }

        [Authorize(Roles = "Pharmacist,Admin")]
        [HttpGet("search")]
        public async Task<IActionResult> SearchPatients(string name)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            List<Patient> patients;

            if (role == "Admin")
            {
                // Admin searches all patients
                patients = await patientRepository.GetPatients();
            }
            else if (role == "Pharmacist")
            {
                // Pharmacist searches only their patients
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var pharmacist = await pharmacistRepository.GetPharmacistByUserId(userId);

                if (pharmacist == null)
                    return NotFound("Pharmacist not found");

                patients = await patientRepository.GetPatientsByPharmacistId(pharmacist.PharmacistId);
            }
            else
            {
                return Forbid();
            }

            var filtered = patients.Where(p =>
                p.FirstName.Contains(name, StringComparison.OrdinalIgnoreCase) ||
                p.LastName.Contains(name, StringComparison.OrdinalIgnoreCase) ||
                (p.FirstName + " " + p.LastName).Contains(name, StringComparison.OrdinalIgnoreCase)
            ).ToList();

            return Ok(filtered);
        }

        [Authorize(Roles = "Pharmacist")]
        [HttpPost]
        public async Task<IActionResult> AddPatient(Patient patient)
        {
            // Automatically assign patient to the logged-in pharmacist
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var pharmacist = await pharmacistRepository.GetPharmacistByUserId(userId);

            if (pharmacist == null)
                return NotFound("Pharmacist not found");

            patient.PharmacistId = pharmacist.PharmacistId;

            var addedPatient = await patientRepository.AddPatient(patient);
            return CreatedAtAction(nameof(GetPatientById), new { id = addedPatient.PatientId }, addedPatient);
        }

        [Authorize(Roles = "Pharmacist,Admin")]
        [HttpPut]
        public async Task<IActionResult> UpdatePatient(Patient patient)
        {
            var existingPatient = await patientRepository.GetPatientById(patient.PatientId);
            if (existingPatient == null)
                return NotFound();

            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            // If pharmacist, check they own this patient
            if (role == "Pharmacist")
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var pharmacist = await pharmacistRepository.GetPharmacistByUserId(userId);

                if (existingPatient.PharmacistId != pharmacist.PharmacistId)
                    return Forbid();
            }

            var updated = await patientRepository.UpdatePatient(patient);
            if (!updated)
                return NotFound();
            return NoContent();
        }

        [Authorize(Roles = "Pharmacist,Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var patient = await patientRepository.GetPatientById(id);
            if (patient == null)
                return NotFound();

            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            // If pharmacist, check they own this patient
            if (role == "Pharmacist")
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var pharmacist = await pharmacistRepository.GetPharmacistByUserId(userId);

                if (patient.PharmacistId != pharmacist.PharmacistId)
                    return Forbid();
            }

            var result = await patientRepository.DeletePatient(id);
            if (result)
                return NoContent();
            return NotFound("erreur de suppression");
        }
    }
}