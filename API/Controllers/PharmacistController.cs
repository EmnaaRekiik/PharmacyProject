using API.Data;

using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using pharmacy;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PharmacistController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IPharmacistRepository pharmacistRepository;
        private readonly IPrescriptionRepository prescriptionRepository;
        private readonly IDispensedMedicationRepository dispensedMedicationRepository;
        private readonly IPatientRepository patientRepository;

        public PharmacistController(
            UserManager<ApplicationUser> userManager,
            IPharmacistRepository pharmacistRepository,
            IPrescriptionRepository prescriptionRepository,
            IDispensedMedicationRepository dispensedMedicationRepository,
            IPatientRepository patientRepository)
        {
            this.userManager = userManager;
            this.pharmacistRepository = pharmacistRepository;
            this.prescriptionRepository = prescriptionRepository;
            this.dispensedMedicationRepository = dispensedMedicationRepository;
            this.patientRepository = patientRepository;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create-pharmacist")]
        public async Task<IActionResult> CreatePharmacist(CreatePharmacistDTO dto)
        {
            if (await userManager.FindByNameAsync(dto.username) != null)
                return BadRequest("Username already exists");

            if (await userManager.FindByEmailAsync(dto.email) != null)
                return BadRequest("Email already exists");

            ApplicationUser applicationUser = new ApplicationUser()
            {
                UserName = dto.username,
                Email = dto.email,
                Role = "Pharmacist"
            };

            var result = await userManager.CreateAsync(applicationUser, dto.password);
            if (result.Succeeded)
            {
                var pharmacist = new Pharmacist
                {
                    UserId = applicationUser.Id,
                    PharmacistName = dto.pharmacistName,
                    PharmacyName = dto.pharmacyName,
                    LicenseNumber = dto.licenseNumber,
                    PhoneNumber = dto.phoneNumber
                };
                await pharmacistRepository.AddPharmacist(pharmacist);

                return Ok(new { message = "Pharmacist created successfully", userId = applicationUser.Id });
            }

            return BadRequest(result.Errors);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("pharmacists")]
        public async Task<IActionResult> GetAllPharmacists()
        {
            var pharmacists = await pharmacistRepository.GetPharmacists();
            return Ok(pharmacists);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("pharmacist/{id}")]
        public async Task<IActionResult> DeletePharmacist(int id)
        {
            var pharmacist = await pharmacistRepository.GetPharmacistById(id);
            if (pharmacist == null)
                return NotFound();

            // Check if they have patients
            var patients = await patientRepository.GetPatientsByPharmacistId(id);
            if (patients.Any())
            {
                return BadRequest(new { message = "Cannot delete pharmacist with existing patients. Please reassign or delete patients first." });
            }

            await pharmacistRepository.DeletePharmacist(id);

            var user = await userManager.FindByIdAsync(pharmacist.UserId);
            if (user != null)
            {
                await userManager.DeleteAsync(user);
            }

            return Ok(new { message = "Pharmacist deleted successfully" });
        }

        [Authorize(Roles = "Admin,Pharmacist")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPharmacistById(int id)
        {
            var pharmacist = await pharmacistRepository.GetPharmacistById(id);
            if (pharmacist == null)
                return NotFound();
            return Ok(pharmacist);
        }

        [Authorize(Roles = "Admin,Pharmacist")]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetPharmacistByUserId(string userId)
        {
            var pharmacist = await pharmacistRepository.GetPharmacistByUserId(userId);
            if (pharmacist == null)
                return NotFound();
            return Ok(pharmacist);
        }

        [Authorize(Roles = "Pharmacist")]
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetPharmacistDashboard()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var pharmacist = await pharmacistRepository.GetPharmacistByUserId(userId);
            if (pharmacist == null)
                return NotFound("Pharmacist profile not found");

            var myPrescriptions = await prescriptionRepository.GetPrescriptionsByPharmacistId(pharmacist.PharmacistId);

            var myDispensedMeds = await dispensedMedicationRepository.GetDispensedMedicationsByPharmacistId(pharmacist.PharmacistId);

            var dashboard = new
            {
                pharmacist = pharmacist,
                totalPrescriptions = myPrescriptions.Count,
                totalDispensedMedications = myDispensedMeds.Count,
                recentPrescriptions = myPrescriptions.OrderByDescending(p => p.PrescriptionDate).Take(5),
                recentDispensed = myDispensedMeds.OrderByDescending(d => d.DeliveryDate).Take(5)
            };

            return Ok(dashboard);
        }

        [Authorize(Roles = "Admin,Pharmacist")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdatePharmacist(Pharmacist pharmacist)
        {
            var updated = await pharmacistRepository.UpdatePharmacist(pharmacist);
            if (!updated)
                return NotFound();
            return NoContent();
        }
    }
}