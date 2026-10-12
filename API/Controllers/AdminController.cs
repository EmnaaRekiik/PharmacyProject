using API.Data;
using API.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IDispensedMedicationRepository dispensedMedicationRepository;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            IDispensedMedicationRepository dispensedMedicationRepository)
        {
            this.userManager = userManager;
            this.dispensedMedicationRepository = dispensedMedicationRepository;
        }

        [HttpGet("profile")]
        public async Task<ActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var admin = await userManager.FindByIdAsync(userId);

            if (admin == null)
                return NotFound("User not found");

            var adminDetails = new
            {
                admin.Id,
                admin.UserName,
                admin.Email,
                admin.PhoneNumber,
                admin.EmailConfirmed,
                Role = admin.Role
            };

            return Ok(adminDetails);
        }

        [HttpPut("profile")]
        public async Task<ActionResult> UpdateProfile([FromBody] UpdateProfileDto updatedAdmin)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var admin = await userManager.FindByIdAsync(userId);

            if (admin == null)
                return NotFound("User not found");

            // Update properties
            if (!string.IsNullOrEmpty(updatedAdmin.UserName))
            {
                var existingUser = await userManager.FindByNameAsync(updatedAdmin.UserName);
                if (existingUser != null && existingUser.Id != admin.Id)
                    return BadRequest("Username already taken");

                admin.UserName = updatedAdmin.UserName;
            }

            if (!string.IsNullOrEmpty(updatedAdmin.Email))
            {
                var existingUser = await userManager.FindByEmailAsync(updatedAdmin.Email);
                if (existingUser != null && existingUser.Id != admin.Id)
                    return BadRequest("Email already taken");

                admin.Email = updatedAdmin.Email;
            }

            if (!string.IsNullOrEmpty(updatedAdmin.PhoneNumber))
                admin.PhoneNumber = updatedAdmin.PhoneNumber;

            var result = await userManager.UpdateAsync(admin);

            if (result.Succeeded)
                return Ok("Profile updated successfully");

            return BadRequest(result.Errors);
        }

        [HttpGet("dispensed-medications")]
        public async Task<ActionResult> GetAllDispensedMedications()
        {
            var dispensedMedications = await dispensedMedicationRepository.GetDispensedMedications();
            return Ok(dispensedMedications);
        }

        [HttpGet("dispensed-medications/today")]
        public async Task<ActionResult> GetTodayDispensedMedications()
        {
            var allDispensed = await dispensedMedicationRepository.GetDispensedMedications();
            var today = allDispensed
                .Where(d => d.DeliveryDate.Date == DateTime.Today)
                .ToList();
            return Ok(today);
        }
    }

    // DTO for profile updates
    public class UpdateProfileDto
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}